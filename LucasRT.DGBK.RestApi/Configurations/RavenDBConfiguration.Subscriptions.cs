using LeadSoft.Common.Library.Constants;
using LeadSoft.Common.Library.EnvUtils;
using LeadSoft.Common.Library.Extensions;
using LucasRT.DGBK.RestApi.Application.Contracts.Webhooks;
using LucasRT.DGBK.RestApi.Domain;
using LucasRT.DGBK.RestApi.Domain.Entities.Payments;
using LucasRT.DGBK.RestApi.Domain.Entities.Refunds;
using LucasRT.DGBK.RestApi.Infrastructure;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using Raven.Client.Documents.Subscriptions;
using Raven.Client.Exceptions.Documents.Subscriptions;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Configurations
{
    /// <summary>
    /// Provides configuration methods for setting up RavenDB subscriptions for processing payments and refunds.
    /// </summary>
    /// <remarks>This static class contains methods to add and manage subscriptions in a RavenDB document store for
    /// handling payment and refund events. It includes constants for webhook headers and subscription names used in the
    /// processing logic. The class ensures that necessary subscriptions are created if they do not already exist and
    /// manages the execution of subscription workers. <see cref="https://docs.ravendb.net/7.1/studio/database/tasks/ongoing-tasks/subscription-task/"/></remarks>
    public static partial class RavenDBConfiguration
    {
        public const string PaymentsWorkerSubscriptionName = "PaymentsWorker";
        public const string RefundsWorkerSubscriptionName = "RefundsWorker";
        public const string WebhookHeader_DeliveryId = "X-Delivery-Id";
        public const string WebhookHeader_Signature = "X-Signature";
        public const string WebhookHeader_Timestamp = "X-Timestamp";

        /// <summary>
        /// Adds a subscription for processing payment records to the service collection.
        /// </summary>
        /// <remarks>This method checks for the existence of a subscription named "PaymentsWorker" in the
        /// RavenDB document store. If the subscription does not exist, it creates a new one with a filter to process
        /// payments with a status of either <see cref="PaymentStatus.Created"/> or <see cref="PaymentStatus.Failed"/>
        /// and a <c>NextAttemptAt</c> date that is either null or in the past.</remarks>
        /// <param name="service">The service collection to which the payment subscription is added.</param>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException">Thrown if the RavenDB document store is not found in the service collection.</exception>
        public static async Task AddPaymentsSubscription(this IServiceCollection service)
        {
            IDocumentStore ravenDB = service.BuildServiceProvider().GetService<IDocumentStore>() ?? throw new OperationCanceledException("RavenDB Document Store not found!");

            try
            {
                await ravenDB.Subscriptions.GetSubscriptionStateAsync(PaymentsWorkerSubscriptionName);
            }
            catch (SubscriptionDoesNotExistException)
            {
                DateTimeOffset now = DateTimeOffset.UtcNow;
                await ravenDB.Subscriptions.CreateAsync(new SubscriptionCreationOptions<Payment>()
                {
                    Name = PaymentsWorkerSubscriptionName,
                    Filter = p => (p.Status == PaymentStatus.Created || p.Status == PaymentStatus.Failed) &&
                                  (p.NextAttemptAt == null || p.NextAttemptAt <= now)
                });
            }

            PaymentsSubcriptionWorker(ravenDB).ConfigureAwait(false);
        }

        /// <summary>
        /// Processes payments using a subscription worker that listens for payment events from a RavenDB instance.
        /// </summary>
        /// <remarks>This method sets up a subscription worker to process payment events. For each payment
        /// event, it sends a POST request to a configured webhook endpoint with the payment details. If the request is
        /// successful, the payment is captured; otherwise, a retry is scheduled. The method handles exceptions by
        /// scheduling a retry for the payment.</remarks>
        /// <param name="ravenDB">The RavenDB document store used to access the subscription worker.</param>
        /// <returns></returns>
        public static async Task PaymentsSubcriptionWorker(IDocumentStore ravenDB)
        {
            SubscriptionWorker<Payment> subscription = ravenDB.Subscriptions.GetSubscriptionWorker<Payment>(new SubscriptionWorkerOptions(PaymentsWorkerSubscriptionName));
            await subscription.Run(async batch =>
            {
                using IAsyncDocumentSession session = batch.OpenAsyncSession();
                using HttpClient httpClient = new()
                {
                    BaseAddress = new(EnvUtil.Get(EnvConstant.Webhook_Endpoint))
                };

                foreach (Payment payment in batch.Items.Select(i => i.Result))
                {
                    DtoProcessPaymentRequest dtoRequest = new()
                    {
                        DtoPayment = new(payment.AccountId, payment.PixKey, payment.Amount)
                    };

                    using HttpRequestMessage request = new(HttpMethod.Post, "webhooks/payment")
                    {
                        Content = new StringContent(dtoRequest.ToJson(), System.Text.Encoding.UTF8, Constant.ApplicationJson)
                    };

                    request.Headers.Add(WebhookHeader_DeliveryId, payment.Id.ToString());
                    request.Headers.Add(WebhookHeader_Signature, $"sha256={dtoRequest.DtoPayment.ToJson().SignHmac(out long timestamp)}");
                    request.Headers.Add(WebhookHeader_Timestamp, timestamp.ToString());

                    PaymentStatusHistory psh;

                    try
                    {
                        HttpResponseMessage response = await httpClient.SendAsync(request);
                        if ((int)response.StatusCode is >= 200 and < 300)
                            psh = payment.Capture();
                        else
                            psh = PaymentStatusHistory.SchedulePaymentRetry(payment, await response.Content.ReadAsStringAsync());
                    }
                    catch (Exception ex)
                    {
                        psh = PaymentStatusHistory.SchedulePaymentRetry(payment, ex.Message);
                    }

                    await session.StoreAsync(payment, payment.Id);
                }

                await session.SaveChangesAsync();
            });
        }

        /// <summary>
        /// Adds a subscription for processing refund events to the service collection.
        /// </summary>
        /// <remarks>This method checks for the existence of a subscription named "RefundsWorker" in the
        /// RavenDB document store. If the subscription does not exist, it creates a new one with specified filtering
        /// and inclusion criteria.</remarks>
        /// <param name="service">The service collection to which the refund subscription will be added.</param>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException">Thrown if the RavenDB Document Store is not found in the service collection.</exception>
        public static async Task AddRefundsSubscription(this IServiceCollection service)
        {
            IDocumentStore ravenDB = service.BuildServiceProvider().GetService<IDocumentStore>() ?? throw new OperationCanceledException("RavenDB Document Store not found!");

            try
            {
                await ravenDB.Subscriptions.GetSubscriptionStateAsync(RefundsWorkerSubscriptionName);
            }
            catch (SubscriptionDoesNotExistException)
            {
                await ravenDB.Subscriptions.CreateAsync(new SubscriptionCreationOptions<Refund>()
                {
                    Name = RefundsWorkerSubscriptionName,
                    //Includes = builder => builder.IncludeDocuments(r => r.PaymentId),
                    Filter = r => (r.Status == RefundStatus.Created || r.Status == RefundStatus.Failed)
                });
            }

            RefundsSubcriptionWorker(ravenDB).ConfigureAwait(false);
        }

        /// <summary>
        /// Processes refund subscriptions by executing refund operations for payments that meet specific criteria.
        /// </summary>
        /// <remarks>This method runs a subscription worker that listens for refund events and processes
        /// them by sending HTTP requests to a configured webhook endpoint. It updates the status of refunds and
        /// payments based on the response from the webhook. If a refund fails, it schedules a retry.</remarks>
        /// <param name="ravenDB">The document store used to access the database for retrieving and updating refund and payment information.</param>
        /// <returns></returns>
        public static async Task RefundsSubcriptionWorker(IDocumentStore ravenDB)
        {
            SubscriptionWorker<Refund> subscription = ravenDB.Subscriptions.GetSubscriptionWorker<Refund>(new SubscriptionWorkerOptions(RefundsWorkerSubscriptionName));
            await subscription.Run(async batch =>
            {
                DateTimeOffset now = DateTimeOffset.UtcNow;
                using IAsyncDocumentSession session = batch.OpenAsyncSession();
                using HttpClient httpClient = new()
                {
                    BaseAddress = new(EnvUtil.Get(EnvConstant.Webhook_Endpoint))
                };

                IEnumerable<string> refundPaymentIds = batch.Items.Select(i => i.Result.PaymentId.GetString()).Distinct();

                IList<Payment> payments = await session.Query<Payment>()
                                                            .Where(p => (p.Id.In(refundPaymentIds)) &&
                                                                        (p.Amount != p.RefundedAmount) &&
                                                                        (p.NextAttemptAt == null || p.NextAttemptAt <= now))
                                                            .ToListAsync();

                IEnumerable<Guid> paymentIds = payments.Where(p => (p.Id.In(refundPaymentIds)) &&
                                                                   (p.Amount != p.RefundedAmount) &&
                                                                   (p.NextAttemptAt == null || p.NextAttemptAt <= now))
                                                       .Select(p => p.Id.ToGuid())
                                                       .Distinct();

                IList<Refund> refunds = batch.Items.Where(i => i.Result.PaymentId.In(paymentIds))
                                                   .Select(i => i.Result)
                                                   .ToList();

                foreach (Refund refund in refunds)
                {
                    Payment payment = payments.Single(p => p.Id == refund.PaymentId.GetString());

                    DtoProcessRefundRequest dtoRequest = new()
                    {
                        DtoRefund = new(refund.Id.ToGuid(), refund.PaymentId, payment.PixKey, refund.Amount)
                    };

                    using HttpRequestMessage request = new(HttpMethod.Post, "webhooks/refund")
                    {
                        Content = new StringContent(dtoRequest.ToJson(), System.Text.Encoding.UTF8, Constant.ApplicationJson)
                    };

                    request.Headers.Add(WebhookHeader_DeliveryId, refund.Id.ToString());
                    request.Headers.Add(WebhookHeader_Signature, $"sha256={dtoRequest.DtoRefund.ToJson().SignHmac(out long timestamp)}");
                    request.Headers.Add(WebhookHeader_Timestamp, timestamp.ToString());

                    PaymentStatusHistory psh;

                    try
                    {
                        HttpResponseMessage response = await httpClient.SendAsync(request);
                        if ((int)response.StatusCode is >= 200 and < 300)
                            psh = payment.Refund(refund.MarkSucceeded());
                        else
                        {
                            refund.MarkFailed();
                            psh = PaymentStatusHistory.ScheduleRefundRetry(payment, await response.Content.ReadAsStringAsync());
                        }
                    }
                    catch (Exception ex)
                    {
                        refund.MarkFailed();
                        psh = PaymentStatusHistory.ScheduleRefundRetry(payment, ex.Message);
                    }

                    await session.StoreAsync(refund, refund.Id);
                    await session.StoreAsync(payment, payment.Id);
                }

                await session.SaveChangesAsync();
            });
        }
    }
}

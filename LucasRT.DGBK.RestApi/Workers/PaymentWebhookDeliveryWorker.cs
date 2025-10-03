using LeadSoft.Common.Library.Constants;
using LeadSoft.Common.Library.EnvUtils;
using LeadSoft.Common.Library.Extensions;
using LucasRT.DGBK.RestApi.Application.Contracts.Webhooks;
using LucasRT.DGBK.RestApi.Domain;
using LucasRT.DGBK.RestApi.Domain.Entities.Payments;
using LucasRT.DGBK.RestApi.Infrastructure;
using LucasRT.DGBK.RestApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Workers
{
    public class PaymentWebhookDeliveryWorker(IConfiguration configuration) : BackgroundService
    {
        private readonly UnitOfWork _UnitOfWork = new(new PostgreSQL(configuration));

        public const string WebhookHeader_DeliveryId = "X-Delivery-Id";
        public const string WebhookHeader_Signature = "X-Signature";
        public const string WebhookHeader_Timestamp = "X-Timestamp";

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                DateTimeOffset now = DateTimeOffset.UtcNow;
                IList<Payment> payments = await _UnitOfWork.GetDbContext().Payments
                                                            .AsTracking()
                                                            .Include(w => w.History)
                                                            .Where(w => (w.Status == PaymentStatus.Created || w.Status == PaymentStatus.Failed) &&
                                                                        (w.NextAttemptAt == null || w.NextAttemptAt <= now))
                                                            .OrderBy(w => w.NextAttemptAt)
                                                            .Take(5)
                                                            .ToListAsync(ct);

                if (payments.Any())
                {
                    using HttpClient httpClient = new()
                    {
                        BaseAddress = new(EnvUtil.Get(EnvConstant.Webhook_Endpoint))
                    };

                    foreach (Payment payment in payments)
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
                            HttpResponseMessage response = await httpClient.SendAsync(request, ct);
                            if ((int)response.StatusCode is >= 200 and < 300)
                                psh = payment.Capture();
                            else
                                psh = PaymentStatusHistory.SchedulePaymentRetry(payment, await response.Content.ReadAsStringAsync(ct));
                        }
                        catch (Exception ex)
                        {
                            psh = PaymentStatusHistory.SchedulePaymentRetry(payment, ex.Message);
                        }

                        await _UnitOfWork.GetDbContext().PaymentStatusHistories.AddAsync(psh);
                    }

                    await _UnitOfWork.CommitDataContextAsync(ct);
                }

                await Task.Delay(TimeSpan.FromSeconds(2), ct);
            }
        }
    }
}

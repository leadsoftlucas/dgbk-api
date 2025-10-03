using LeadSoft.Common.Library.Constants;
using LeadSoft.Common.Library.EnvUtils;
using LeadSoft.Common.Library.Extensions;
using LucasRT.DGBK.RestApi.Application.Contracts.Webhooks;
using LucasRT.DGBK.RestApi.Domain;
using LucasRT.DGBK.RestApi.Domain.Entities.Payments;
using LucasRT.DGBK.RestApi.Domain.Entities.Refunds;
using LucasRT.DGBK.RestApi.Infrastructure;
using LucasRT.DGBK.RestApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Workers
{
    public class RefundWebhookDeliveryWorker(IConfiguration configuration) : BackgroundService
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
                IList<Refund> refunds = await _UnitOfWork.GetDbContext().Refunds
                                                            .AsTracking()
                                                            .Include(r => r.Payment)
                                                            .Include(w => w.Payment.History)
                                                            .Where(w => (w.Status == RefundStatus.Created || w.Status == RefundStatus.Failed) &&
                                                                        (w.Payment.Amount != w.Payment.RefundedAmount) &&
                                                                        (w.Payment.NextAttemptAt == null || w.Payment.NextAttemptAt <= now))
                                                            .Take(5)
                                                            .ToListAsync(ct);

                if (refunds.Any())
                {
                    using HttpClient httpClient = new()
                    {
                        BaseAddress = new(EnvUtil.Get(EnvConstant.Webhook_Endpoint))
                    };

                    foreach (Refund refund in refunds)
                    {
                        DtoProcessRefundRequest dtoRequest = new()
                        {
                            DtoRefund = new(refund.Id, refund.PaymentId, refund.Payment.PixKey, refund.Amount)
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
                            HttpResponseMessage response = await httpClient.SendAsync(request, ct);
                            if ((int)response.StatusCode is >= 200 and < 300)
                                psh = refund.Payment.Refund(refund.MarkSucceeded());
                            else
                                psh = PaymentStatusHistory.ScheduleRefundRetry(refund.MarkFailed().Payment, await response.Content.ReadAsStringAsync(ct));
                        }
                        catch (Exception ex)
                        {
                            psh = PaymentStatusHistory.ScheduleRefundRetry(refund.MarkFailed().Payment, ex.Message);
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

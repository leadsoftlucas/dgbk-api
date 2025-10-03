using LucasRT.DGBK.RestApi.Infrastructure.Data;

namespace LucasRT.DGBK.RestApi.Workers
{
    public class WebhookDeliveryWorker(IUnitOfWork unitOfWork/*, IHttpClientFactory_http, IOptions<WebhookOptions> opt*/) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var now = DateTimeOffset.UtcNow;
                //var batch = await unitOfWork.WebhookDeliveries
                //    .Where(w => w.Status == "PENDING" && w.NextAttemptAt <= now && w.DeadlineAt > now)
                //    .OrderBy(w => w.NextAttemptAt).Take(100).ToListAsync(ct);

                //foreach (var w in batch)
                //{
                //    var ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                //    var sig = HmacSigner.Sign(_opt.Value.Secret, ts, w.PayloadJson);
                //    using var req = new HttpRequestMessage(HttpMethod.Post, w.TargetUrl)
                //    {
                //        Content = new StringContent(w.PayloadJson, System.Text.Encoding.UTF8, "application/json")
                //    };
                //    req.Headers.Add("X-Timestamp", ts);
                //    req.Headers.Add("X-Event-Type", w.EventType);
                //    req.Headers.Add("X-Delivery-Id", w.Id.ToString());
                //    req.Headers.Add("X-Signature", $"sha256={sig}");

                //    var client = _http.CreateClient("webhooks");
                //    try
                //    {
                //        var resp = await client.SendAsync(req, ct);
                //        if ((int)resp.StatusCode is >= 200 and < 300)
                //        {
                //            w.Status = "DELIVERED"; w.DeliveredAt = DateTimeOffset.UtcNow;
                //        }
                //        else ScheduleRetry(w);
                //    }
                //    catch (Exception ex)
                //    {
                //        w.LastError = ex.Message; ScheduleRetry(w);
                //    }
                //}

                //await _db.SaveChangesAsync(ct);
                await Task.Delay(TimeSpan.FromSeconds(2), ct);
            }
        }

        private static void ScheduleRetry(/*WebhookDelivery w*/)
        {
            //w.AttemptCount++;
            //var baseDelay = TimeSpan.FromSeconds(30);
            //var cap = TimeSpan.FromMinutes(30);
            //var delay = TimeSpan.FromSeconds(Math.Min(baseDelay.TotalSeconds * Math.Pow(2, w.AttemptCount), cap.TotalSeconds));
            //w.NextAttemptAt = DateTimeOffset.UtcNow + delay;
            //if (w.NextAttemptAt > w.DeadlineAt) { w.Status = "FAILED_FINAL"; }
        }
    }
}

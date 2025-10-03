using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Domain.Entities.Payments
{
    public partial class PaymentStatusHistory
    {
        public PaymentStatusHistory()
        {
        }

        public static PaymentStatusHistory SchedulePaymentRetry(Payment payment, string reason)
        {
            TimeSpan baseDelay = TimeSpan.FromSeconds(5);
            TimeSpan cap = TimeSpan.FromMinutes(5);
            TimeSpan delay = TimeSpan.FromSeconds(Math.Min(baseDelay.TotalSeconds * Math.Pow(2, payment.History.Count(h => h.Status == PaymentStatus.Failed)), cap.TotalSeconds));

            return payment.MarkPaymentFailed(reason, DateTimeOffset.UtcNow.Add(delay));
        }

        public static PaymentStatusHistory ScheduleRefundRetry(Payment payment, string reason)
        {
            TimeSpan baseDelay = TimeSpan.FromSeconds(5);
            TimeSpan cap = TimeSpan.FromMinutes(5);
            TimeSpan delay = TimeSpan.FromSeconds(Math.Min(baseDelay.TotalSeconds * Math.Pow(2, payment.History.Count(h => h.Status == PaymentStatus.RefundFailed)), cap.TotalSeconds));

            return payment.MarkRefundFailed(reason, DateTimeOffset.UtcNow.Add(delay));
        }
    }
}

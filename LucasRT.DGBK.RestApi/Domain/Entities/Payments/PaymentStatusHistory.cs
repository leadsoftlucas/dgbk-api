using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Domain.Entities.Payments
{
    public partial class PaymentStatusHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public PaymentStatus Status { get; set; } = PaymentStatus.Created;
        public string? Reason { get; set; } = string.Empty;
        public DateTimeOffset At { get; set; } = DateTimeOffset.UtcNow;
    }
}

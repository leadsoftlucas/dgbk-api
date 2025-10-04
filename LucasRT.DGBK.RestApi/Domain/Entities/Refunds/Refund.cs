using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Domain.Entities.Refunds
{
    public partial class Refund
    {
        public string Id { get; private set; } = string.Empty;
        public Guid PaymentId { get; private set; } = Guid.Empty;
        public decimal Amount { get; private set; } = 0m;
        public RefundStatus Status { get; private set; } = RefundStatus.Created;
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? CompletedAt { get; private set; }
    }
}

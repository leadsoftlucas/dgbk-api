using LucasRT.DGBK.RestApi.Domain.Entities.Payments;
using System.ComponentModel.DataAnnotations.Schema;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Domain.Entities.Refunds
{
    [Table("Refund")]
    public partial class Refund
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid PaymentId { get; private set; }
        public decimal Amount { get; private set; } = 0m;
        public RefundStatus Status { get; private set; } = RefundStatus.Created;

        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? CompletedAt { get; private set; }
        public string? FailureReason { get; private set; }

        public Payment? Payment { get; private set; }
    }
}

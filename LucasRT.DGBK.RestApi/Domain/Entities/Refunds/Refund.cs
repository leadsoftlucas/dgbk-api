using LucasRT.DGBK.RestApi.Domain.Entities.Payments;
using System.ComponentModel.DataAnnotations.Schema;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Domain.Entities.Refunds
{
    [Table("Refunds")]
    public partial class Refund
    {
        public Guid Id { get; private set; } = Guid.Empty;
        public Guid PaymentId { get; private set; } = Guid.Empty;
        public decimal Amount { get; private set; } = 0m;
        public RefundStatus Status { get; private set; } = RefundStatus.Created;
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? CompletedAt { get; private set; }
        public Payment? Payment { get; private set; }
    }
}

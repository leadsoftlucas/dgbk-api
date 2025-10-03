using System.ComponentModel.DataAnnotations.Schema;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Domain.Entities.Payments
{
    [Table("Payments")]
    public partial class Payment
    {
        private readonly List<PaymentStatusHistory> _history = [];

        public Guid Id { get; private set; } = Guid.Empty;
        public Guid TransactionId { get; private set; } = Guid.Empty;
        public Guid AccountId { get; private set; } = Guid.Empty;
        public string PixKey { get; private set; } = string.Empty;
        public decimal Amount { get; private set; } = 0m;
        public decimal RefundedAmount { get; private set; } = 0m;
        public PaymentStatus Status { get; private set; } = PaymentStatus.Created;
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? CapturedAt { get; private set; } = null;
        public DateTimeOffset? NextAttemptAt { get; private set; } = null;
        public DateTimeOffset? DeadlineAt { get; private set; } = null;
        public IReadOnlyCollection<PaymentStatusHistory> History => _history;
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Domain.Entities.Payments
{
    [Table("Payment")]
    public partial class Payment
    {
        private readonly List<PaymentStatusHistory> _history = [];

        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid AccountId { get; private set; } = Guid.NewGuid();
        public string PixKey { get; private set; } = string.Empty;
        public decimal Amount { get; private set; } = 0m;
        public PaymentStatus Status { get; private set; } = PaymentStatus.Created;
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? CapturedAt { get; private set; }
        public IReadOnlyCollection<PaymentStatusHistory> History => _history;
    }
}

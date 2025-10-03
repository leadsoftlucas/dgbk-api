using System.ComponentModel.DataAnnotations.Schema;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Domain.Entities.Payments
{
    [Table("Payment")]
    public partial class PaymentStatusHistory
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public PaymentStatus Status { get; set; }
        public string? Reason { get; set; }
        public DateTimeOffset At { get; set; }

        public Payment? Payment { get; set; }
    }
}

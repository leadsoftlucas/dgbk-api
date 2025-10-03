using LucasRT.DGBK.RestApi.Domain.Entities.Payments;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Domain.Entities.Refunds
{
    public partial class Refund
    {
        public Refund()
        {
        }

        public Refund(Payment payment, decimal amount)
        {
            Id = Guid.NewGuid();
            PaymentId = payment.Id;
            Amount = amount < payment.Amount ? amount : payment.Amount;
            Payment = payment;
        }

        public Refund MarkFailed()
        {
            Status = RefundStatus.Failed;

            return this;
        }

        public Refund MarkSucceeded()
        {
            Status = RefundStatus.Succeeded;
            CompletedAt = DateTimeOffset.UtcNow;

            return this;
        }
    }
}

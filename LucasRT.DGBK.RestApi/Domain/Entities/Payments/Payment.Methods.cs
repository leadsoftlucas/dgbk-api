using LeadSoft.Common.Library.Enumerators;
using LucasRT.DGBK.RestApi.Domain.Entities.Refunds;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Domain.Entities.Payments
{
    public partial class Payment
    {
        public Payment()
        {
        }

        public Payment(string pixKey, decimal amount)
        {
            Id = Guid.NewGuid();
            PixKey = pixKey;
            Amount = amount;

            AppendHistory(PaymentStatus.Created, PaymentStatus.Created.GetDescription());
        }

        public void Capture()
        {
            if (Status is PaymentStatus.Failed or PaymentStatus.Refunded) throw new InvalidOperationException("Invalid state");

            Status = PaymentStatus.Captured;
            CapturedAt = DateTimeOffset.UtcNow;
            AppendHistory(Status, Status.GetDescription());
        }

        public Refund Refund(decimal amount)
        {
            //if (Status is PaymentStatus.Failed) throw new InvalidOperationException("Cannot refund failed payment");
            //if (amount <= 0 || amount + RefundedAmount > Amount.Value) throw new InvalidOperationException("Invalid refund amount");

            //RefundedAmount += amount;
            //if (RefundedAmount == Amount.Value) { Status = PaymentStatus.Refunded; AppendHistory(Status, "fully refunded"); }

            //return Refund.Create(this.Id, new Money(amount, Amount.Currency));

            return null;
        }

        public void MarkFailed(string reason)
        {
            Status = PaymentStatus.Failed;
            AppendHistory(Status, reason);
        }

        private void AppendHistory(PaymentStatus status, string? reason)
        {
            _history.Add(new PaymentStatusHistory
            {
                Id = Guid.NewGuid(),
                PaymentId = this.Id,
                Status = status,
                Reason = reason,
                At = DateTimeOffset.UtcNow
            });
        }
    }
}

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

        public Payment(string pixKey, decimal amount, Guid transactionId)
        {
            Id = Guid.NewGuid();
            TransactionId = transactionId;
            PixKey = pixKey;
            Amount = amount;

            AppendHistory(PaymentStatus.Created, PaymentStatus.Created.GetDescription());
        }

        public PaymentStatusHistory Capture()
        {
            if (Status is not PaymentStatus.Created)
                throw new InvalidOperationException("Invalid state");

            Status = PaymentStatus.Captured;
            CapturedAt = DateTimeOffset.UtcNow;
            NextAttemptAt = null;
            DeadlineAt = null;

            return AppendHistory(Status, Status.GetDescription());
        }

        public PaymentStatusHistory Refund(Refund refund)
        {
            if (Status is not PaymentStatus.Captured or PaymentStatus.FullRefunded)
                throw new InvalidOperationException("Cannot refund uncaptured, failed or already fully refunded payment.");

            if (refund.Status is not RefundStatus.Succeeded)
                throw new InvalidOperationException("Cannot refund pending or failed refund.");

            RefundedAmount += refund.Amount;
            Status = RefundedAmount == Amount ? PaymentStatus.FullRefunded : PaymentStatus.PartialRefunded;
            NextAttemptAt = null;
            DeadlineAt = null;

            return AppendHistory(Status, $"{Status.GetDescription()}: Total: {Amount} | Refunded :{RefundedAmount}");
        }

        public PaymentStatusHistory MarkRefundFailed(string reason, DateTimeOffset nextAttempt)
        {
            if (!DeadlineAt.HasValue)
                DeadlineAt = DateTimeOffset.UtcNow.AddHours(24);

            NextAttemptAt = nextAttempt < DeadlineAt ? nextAttempt : DeadlineAt;

            return AppendHistory(PaymentStatus.RefundFailed, reason);
        }

        public PaymentStatusHistory MarkPaymentFailed(string reason, DateTimeOffset nextAttempt)
        {
            if (!DeadlineAt.HasValue)
                DeadlineAt = DateTimeOffset.UtcNow.AddHours(24);

            NextAttemptAt = nextAttempt < DeadlineAt ? nextAttempt : DeadlineAt;

            Status = PaymentStatus.Failed;
            return AppendHistory(Status, reason);
        }

        private PaymentStatusHistory AppendHistory(PaymentStatus status, string? reason)
        {
            PaymentStatusHistory sh = new()
            {
                Id = Guid.NewGuid(),
                PaymentId = this.Id,
                Status = status,
                Reason = reason,
                At = DateTimeOffset.UtcNow
            };

            _history.Add(sh);

            return sh;
        }
    }
}

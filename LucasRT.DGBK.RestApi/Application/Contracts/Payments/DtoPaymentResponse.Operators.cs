using LucasRT.DGBK.RestApi.Domain.Entities.Payments;

namespace LucasRT.DGBK.RestApi.Application.Contracts.Payments
{
    public partial class DtoPaymentResponse
    {
        public static implicit operator DtoPaymentResponse(Payment payment)
        {
            if (payment is null)
                return null!;

            return new DtoPaymentResponse
            {
                Id = payment.Id,
                TransactionId = payment.TransactionId,
                PixKey = payment.PixKey,
                Amount = payment.Amount,
                RefundedAmount = payment.RefundedAmount,
                PaymentStatus = payment.Status,
                CreatedAt = payment.CreatedAt,
                CapturedAt = payment.CapturedAt,
                History = (bool)(payment.History?.Any()) ? payment.History.Select(h => (DtoPaymentHistory)h).ToList() : []
            };
        }
    }
}

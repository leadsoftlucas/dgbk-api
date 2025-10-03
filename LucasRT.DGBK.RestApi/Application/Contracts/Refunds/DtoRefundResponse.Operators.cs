using LucasRT.DGBK.RestApi.Domain.Entities.Refunds;

namespace LucasRT.DGBK.RestApi.Application.Contracts.Refunds
{
    public partial class DtoRefundResponse
    {
        public static implicit operator DtoRefundResponse(Refund refund)
        {
            if (refund is null)
                return null;

            return new DtoRefundResponse
            {
                Id = refund.Id,
                PaymentId = refund.PaymentId,
                CompletedAt = refund.CompletedAt,
                CreatedAt = refund.CreatedAt,
                RefundStatus = refund.Status,
                PaymentAmount = refund.Payment.Amount,
                RefundAmount = refund.Amount,
                RemainingAmount = refund.Payment.Amount - refund.Payment.RefundedAmount,
            };
        }
    }
}

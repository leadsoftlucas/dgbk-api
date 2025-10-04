using LucasRT.DGBK.RestApi.Domain.Entities.Payments;
using LucasRT.DGBK.RestApi.Domain.Entities.Refunds;

namespace LucasRT.DGBK.RestApi.Application.Contracts.Refunds
{
    public partial class DtoRefundResponse
    {
        public DtoRefundResponse SetPayment(Payment payment)
        {
            PaymentAmount = payment.Amount;
            RemainingAmount = payment.Amount - payment.RefundedAmount;

            return this;
        }

        public static DtoRefundResponse ToDto(Refund refund, Payment payment)
        {
            DtoRefundResponse dtoResponse = refund;

            return dtoResponse.SetPayment(payment);
        }

        public static IList<DtoRefundResponse> ToDtos(IList<Refund> refunds, IList<Payment> payments)
        {
            IList<DtoRefundResponse> dtoResponses = [.. refunds.Select(p => (DtoRefundResponse)p)];

            foreach (DtoRefundResponse dto in dtoResponses)
                dto.SetPayment(payments.First(p => p.Id.Equals(dto.PaymentId)));

            return dtoResponses;
        }
    }
}

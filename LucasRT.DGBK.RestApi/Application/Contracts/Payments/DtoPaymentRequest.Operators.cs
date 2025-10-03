using LucasRT.DGBK.RestApi.Domain.Entities.Payments;

namespace LucasRT.DGBK.RestApi.Application.Contracts.Payments
{
    public partial class DtoPaymentRequest
    {
        public static implicit operator Payment(DtoPaymentRequest request)
        {
            return new Payment(request.PixKey, request.Amount);
        }
    }
}

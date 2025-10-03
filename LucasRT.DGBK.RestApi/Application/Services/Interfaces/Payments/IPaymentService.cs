using LucasRT.DGBK.RestApi.Application.Contracts.Payments;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Application.Services.Interfaces.Payments
{
    public interface IPaymentService
    {
        Task<DtoPaymentResponse> CreatePaymentAsync(DtoPaymentRequest request);
        Task<DtoPaymentResponse> GetPaymentAsync(Guid id);
        Task<IList<DtoPaymentResponse>> GetPaymentsAsync(PaymentStatus? paymentStatus = null);
    }
}

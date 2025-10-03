using LucasRT.DGBK.RestApi.Application.Contracts.Payments;

namespace LucasRT.DGBK.RestApi.Application.Services.Interfaces.Payments
{
    public interface IPaymentService
    {
        Task<DtoPaymentResponse> CreatePaymentAsync(DtoPaymentRequest request);
        Task<DtoPaymentResponse?> GetPaymentByIdAsync(Guid id);
    }
}

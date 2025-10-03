using LucasRT.DGBK.RestApi.Application.Contracts.Payments;
using LucasRT.DGBK.RestApi.Application.Services.Interfaces.Payments;
using LucasRT.DGBK.RestApi.Domain.Entities.Payments;
using LucasRT.DGBK.RestApi.Infrastructure.Data;

namespace LucasRT.DGBK.RestApi.Application.Services.Payments
{
    public class PaymentService(IUnitOfWork unitOfWork) : IPaymentService
    {
        public async Task<DtoPaymentResponse> CreatePaymentAsync(DtoPaymentRequest request)
        {
            Payment payment = request;

            

            return payment;
        }

        public async Task<DtoPaymentResponse?> GetPaymentByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}

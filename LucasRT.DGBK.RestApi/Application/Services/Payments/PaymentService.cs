using LucasRT.DGBK.RestApi.Application.Contracts.Payments;
using LucasRT.DGBK.RestApi.Application.Services.Interfaces.Payments;
using LucasRT.DGBK.RestApi.Domain.Entities.Payments;
using LucasRT.DGBK.RestApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Application.Services.Payments
{
    public class PaymentService(IUnitOfWork unitOfWork) : IPaymentService
    {
        public async Task<DtoPaymentResponse> CreatePaymentAsync(DtoPaymentRequest request)
        {
            Payment payment = request;

            await unitOfWork.GetDbContext().Payments.AddAsync(payment);
            await unitOfWork.CommitDataContextAsync();

            return payment;
        }

        public async Task<DtoPaymentResponse> GetPaymentAsync(Guid id)
            => await unitOfWork.GetDbContext().Payments.AsQueryable<Payment>()
                                                       .SingleOrDefaultAsync(b => b.Id == id);

        public async Task<IList<DtoPaymentResponse>> GetPaymentsAsync(PaymentStatus? paymentStatus = null)
        {
            IQueryable<Payment> query = unitOfWork.GetDbContext().Payments.AsQueryable<Payment>()
                                                                          .Include(p => p.History);

            if (paymentStatus.HasValue)
                query = query.Where(p => p.Status == paymentStatus.Value);

            IList<Payment> payments = await query.ToListAsync();

            return [.. payments.Select(p => (DtoPaymentResponse)p)];
        }
    }
}

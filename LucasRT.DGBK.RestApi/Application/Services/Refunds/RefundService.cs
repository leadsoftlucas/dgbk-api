using LucasRT.DGBK.RestApi.Application.Contracts.Refunds;
using LucasRT.DGBK.RestApi.Application.Services.Interfaces.Refunds;
using LucasRT.DGBK.RestApi.Domain.Entities.Payments;
using LucasRT.DGBK.RestApi.Domain.Entities.Refunds;
using LucasRT.DGBK.RestApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Application.Services.Refunds
{
    public class RefundService(IUnitOfWork unitOfWork) : IRefundService
    {
        public async Task<DtoRefundResponse> CreateRefundAsync(DtoRefundRequest request)
        {
            Payment payment = await unitOfWork.GetDbContext().Payments.AsQueryable<Payment>()
                                                                      .AsTracking()
                                                                      .SingleOrDefaultAsync(b => b.Id == request.PaymentId);

            if (payment is null)
                return null;

            Refund refund = new(payment, request.Amount);

            await unitOfWork.GetDbContext().Refunds.AddAsync(refund);
            await unitOfWork.CommitDataContextAsync();

            return refund;
        }

        public async Task<DtoRefundResponse> GetRefundAsync(Guid id)
            => await unitOfWork.GetDbContext().Refunds.AsQueryable<Refund>()
                                                      .Include(r => r.Payment)
                                                      .Include(r => r.Payment.History)
                                                      .SingleOrDefaultAsync(b => b.Id == id);

        public async Task<IList<DtoRefundResponse>> GetRefundsAsync(RefundStatus? refundStatus = null)
        {
            IQueryable<Refund> query = unitOfWork.GetDbContext().Refunds.AsQueryable<Refund>()
                                                                        .Include(r => r.Payment)
                                                                        .Include(r => r.Payment.History);

            if (refundStatus.HasValue)
                query = query.Where(p => p.Status == refundStatus.Value);

            IList<Refund> refunds = await query.ToListAsync();

            return [.. refunds.Select(p => (DtoRefundResponse)p)];
        }
    }
}

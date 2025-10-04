using LeadSoft.Common.Library.Extensions;
using LucasRT.DGBK.RestApi.Application.Contracts.Refunds;
using LucasRT.DGBK.RestApi.Application.Services.Interfaces.Refunds;
using LucasRT.DGBK.RestApi.Domain.Entities.Payments;
using LucasRT.DGBK.RestApi.Domain.Entities.Refunds;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Application.Services.Refunds
{
    public class RefundService(IDocumentStore ravendb) : IRefundService
    {
        public async Task<DtoRefundResponse> CreateRefundAsync(DtoRefundRequest request)
        {
            IAsyncDocumentSession _session = ravendb.OpenAsyncSession();

            Payment payment = await _session.Query<Payment>()
                                            .SingleOrDefaultAsync(b => b.Id.Equals(request.PaymentId.GetString()));

            if (payment is null)
                return null;

            Refund refund = new(payment, request.Amount);

            await _session.StoreAsync(refund);
            await _session.SaveChangesAsync();

            return DtoRefundResponse.ToDto(refund, payment);
        }

        public async Task<DtoRefundResponse> GetRefundAsync(Guid id)
        {
            IAsyncDocumentSession _session = ravendb.OpenAsyncSession();

            Refund refund = await _session.Query<Refund>()
                                          .Include(r => r.PaymentId)
                                          .SingleOrDefaultAsync(b => b.Id.Equals(id.GetString()));
            if (refund is null)
                return null;

            Payment payment = await _session.LoadAsync<Payment>(refund.PaymentId.GetString());

            return DtoRefundResponse.ToDto(refund, payment);
        }

        public async Task<IList<DtoRefundResponse>> GetRefundsAsync(RefundStatus? refundStatus = null)
        {
            IAsyncDocumentSession _session = ravendb.OpenAsyncSession();

            IRavenQueryable<Refund> query = _session.Query<Refund>()
                                                    .Include(r => r.PaymentId);

            if (refundStatus.HasValue)
                query = query.Where(p => p.Status == refundStatus.Value);

            IList<Refund> refunds = await query.ToListAsync();

            IList<Payment> payments = await _session.Query<Payment>()
                                                    .Where(p => p.Id.In(refunds.Select(r => r.PaymentId.GetString()).Distinct()))
                                                    .ToListAsync();

            return DtoRefundResponse.ToDtos(refunds, payments);
        }
    }
}

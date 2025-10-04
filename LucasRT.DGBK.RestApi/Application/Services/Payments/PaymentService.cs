using LeadSoft.Common.Library.Extensions;
using LucasRT.DGBK.RestApi.Application.Contracts.Payments;
using LucasRT.DGBK.RestApi.Application.Services.Interfaces.Payments;
using LucasRT.DGBK.RestApi.Domain.Entities.Payments;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Application.Services.Payments
{
    public class PaymentService(IDocumentStore ravendb) : IPaymentService
    {
        public async Task<DtoPaymentResponse> CreatePaymentAsync(DtoPaymentRequest request)
        {
            IAsyncDocumentSession _session = ravendb.OpenAsyncSession();
            Payment payment = request;

            await _session.StoreAsync(payment);
            await _session.SaveChangesAsync();

            return payment;
        }

        public async Task<DtoPaymentResponse> GetPaymentAsync(Guid id)
            => await ravendb.OpenAsyncSession().Query<Payment>()
                                               .SingleOrDefaultAsync(b => b.Id.Equals(id.GetString()));

        public async Task<IList<DtoPaymentResponse>> GetPaymentsAsync(PaymentStatus? paymentStatus = null)
        {
            IQueryable<Payment> query = ravendb.OpenAsyncSession().Query<Payment>();

            if (paymentStatus.HasValue)
                query = query.Where(p => p.Status == paymentStatus.Value);

            IList<Payment> payments = await query.ToListAsync();

            return [.. payments.Select(p => (DtoPaymentResponse)p)];
        }
    }
}

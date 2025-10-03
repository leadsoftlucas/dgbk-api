using LucasRT.DGBK.RestApi.Application.Contracts.Refunds;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Application.Services.Interfaces.Refunds
{
    public interface IRefundService
    {
        Task<DtoRefundResponse> CreateRefundAsync(DtoRefundRequest request);
        Task<DtoRefundResponse> GetRefundAsync(Guid id);
        Task<IList<DtoRefundResponse>> GetRefundsAsync(RefundStatus? refundStatus = null);
    }
}

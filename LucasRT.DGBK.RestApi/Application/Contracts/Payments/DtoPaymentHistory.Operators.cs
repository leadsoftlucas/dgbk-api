using LucasRT.DGBK.RestApi.Domain.Entities.Payments;

namespace LucasRT.DGBK.RestApi.Application.Contracts.Payments
{
    public partial class DtoPaymentHistory
    {
        public static implicit operator DtoPaymentHistory(PaymentStatusHistory paymentHistory)
        {
            if (paymentHistory is null)
                return null!;

            return new DtoPaymentHistory
            {
                Id = paymentHistory.Id,
                PaymentStatus = paymentHistory.Status,
                At = paymentHistory.At,
                Reason = paymentHistory.Reason
            };
        }
    }
}

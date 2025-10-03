using System.Runtime.Serialization;

namespace LucasRT.DGBK.RestApi.Application.Contracts.Webhooks
{
    [Serializable]
    [DataContract]
    public partial class DtoRefundProcess
    {
        [DataMember]
        public Guid RefundId { get; private set; } = Guid.Empty;

        [DataMember]
        public Guid PaymentId { get; private set; } = Guid.Empty;

        [DataMember]
        public string PixKey { get; private set; } = string.Empty;

        [DataMember]
        public decimal Amount { get; private set; } = 0m;

        public DtoRefundProcess()
        {
        }

        public DtoRefundProcess(Guid refundId, Guid paymentId, string pixKey, decimal amount)
        {
            RefundId = refundId;
            PaymentId = paymentId;
            PixKey = pixKey;
            Amount = amount;
        }
    }
}

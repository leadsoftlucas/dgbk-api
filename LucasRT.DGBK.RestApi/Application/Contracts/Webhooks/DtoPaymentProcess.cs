using System.Runtime.Serialization;

namespace LucasRT.DGBK.RestApi.Application.Contracts.Webhooks
{
    [Serializable]
    [DataContract]
    public partial class DtoPaymentProcess
    {
        [DataMember]
        public Guid AccountId { get; private set; } = Guid.Empty;

        [DataMember]
        public string PixKey { get; private set; } = string.Empty;

        [DataMember]
        public decimal Amount { get; private set; } = 0m;

        public DtoPaymentProcess()
        {
        }

        public DtoPaymentProcess(Guid accountId, string pixKey, decimal amount)
        {
            AccountId = accountId;
            PixKey = pixKey;
            Amount = amount;
        }
    }
}

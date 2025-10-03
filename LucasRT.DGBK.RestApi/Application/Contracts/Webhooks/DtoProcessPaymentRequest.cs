using System.Runtime.Serialization;

namespace LucasRT.DGBK.RestApi.Application.Contracts.Webhooks
{
    [Serializable]
    [DataContract]
    public partial class DtoProcessPaymentRequest
    {
        [DataMember]
        public DtoPaymentProcess DtoPayment { get; set; }
    }
}

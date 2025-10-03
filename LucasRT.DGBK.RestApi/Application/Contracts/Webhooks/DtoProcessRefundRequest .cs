using System.Runtime.Serialization;

namespace LucasRT.DGBK.RestApi.Application.Contracts.Webhooks
{
    [Serializable]
    [DataContract]
    public partial class DtoProcessRefundRequest
    {
        [DataMember]
        public DtoRefundProcess DtoRefund { get; set; }
    }
}

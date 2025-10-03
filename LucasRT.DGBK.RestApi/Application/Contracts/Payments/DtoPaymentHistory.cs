using System.Runtime.Serialization;
using static LeadSoft.Common.Library.Enumerators.Enums;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Application.Contracts.Payments
{
    [Serializable]
    [DataContract]
    public partial class DtoPaymentHistory
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public PaymentStatus PaymentStatus { get; set; }

        [DataMember]
        public DTOEnumContent Status { get => PaymentStatus.ToDto(); }

        [DataMember]
        public string? Reason { get; set; }

        [DataMember]
        public DateTimeOffset At { get; set; }
    }
}

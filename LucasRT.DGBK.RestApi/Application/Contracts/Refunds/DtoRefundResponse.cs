using LeadSoft.Common.Library.Enumerators;
using System.Runtime.Serialization;
using static LeadSoft.Common.Library.Enumerators.Enums;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Application.Contracts.Refunds
{
    [Serializable]
    [DataContract]
    public partial class DtoRefundResponse
    {
        [DataMember]
        public Guid Id { get; private set; }

        [DataMember]
        public Guid PaymentId { get; private set; }

        [DataMember]
        public decimal PaymentAmount { get; private set; }

        [DataMember]
        public decimal RefundAmount { get; private set; }

        [DataMember]
        public decimal RemainingAmount { get; private set; }

        [DataMember]
        public DateTimeOffset CreatedAt { get; private set; }

        [DataMember]
        public DateTimeOffset? CompletedAt { get; private set; }

        [DataMember]
        public RefundStatus RefundStatus { get; private set; }

        [DataMember]
        public DTOEnumContent Status { get => RefundStatus.ToDto(); }
    }
}

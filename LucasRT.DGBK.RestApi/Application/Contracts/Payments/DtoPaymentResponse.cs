using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;
using static LeadSoft.Common.Library.Enumerators.Enums;
using static LucasRT.DGBK.RestApi.Domain.ValuedObjects.Enums;

namespace LucasRT.DGBK.RestApi.Application.Contracts.Payments
{
    [Serializable]
    [DataContract]
    public partial class DtoPaymentResponse
    {
        [DataMember]
        public Guid Id { get; private set; } = Guid.Empty;

        [DataMember]
        public Guid TransactionId { get; private set; } = Guid.Empty;

        [DataMember]
        public string PixKey { get; private set; } = string.Empty;

        [DataMember]
        public decimal Amount { get; private set; } = 0m;

        [DataMember]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(0)]
        public decimal RefundedAmount { get; private set; } = 0m;

        [DataMember]
        public decimal? RemainingAmount { get => RefundedAmount != 0 ? Amount - RefundedAmount : null; }

        [DataMember]
        public PaymentStatus PaymentStatus { get; private set; } = PaymentStatus.Created;

        [DataMember]
        public DTOEnumContent Status { get => PaymentStatus.ToDto(); }

        [DataMember]
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

        [DataMember]
        public DateTimeOffset? CapturedAt { get; private set; }

        [DataMember]
        public IList<DtoPaymentHistory> History { get; private set; } = [];
    }
}

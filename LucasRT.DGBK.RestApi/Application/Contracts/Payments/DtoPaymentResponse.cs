using System.Runtime.Serialization;
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
        public string PixKey { get; private set; } = string.Empty;

        [DataMember]
        public decimal Amount { get; private set; } = 0m;

        [DataMember]
        public PaymentStatus Status { get; private set; } = PaymentStatus.Created;

        [DataMember]
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

        [DataMember]
        public DateTimeOffset? CapturedAt { get; private set; }
    }
}

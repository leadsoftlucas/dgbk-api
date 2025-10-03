using LeadSoft.Common.Library.Constants;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace LucasRT.DGBK.RestApi.Application.Contracts.Refunds
{
    [Serializable]
    [DataContract]
    public class DtoRefundRequest
    {
        [DataMember]
        [Required(ErrorMessage = Constant.RequiredField)]
        public Guid PaymentId { get; set; } = Guid.Empty;

        [DataMember]
        [Required(ErrorMessage = Constant.RequiredField)]
        [Range(0.01, 10000000.00, ErrorMessage = "{0} value must be greater than {1} and lower than {2}.")]
        [DefaultValue(100)]
        public decimal Amount { get; set; } = 0m;
    }
}

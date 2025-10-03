using LeadSoft.Common.Library.Constants;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace LucasRT.DGBK.RestApi.Application.Contracts.Payments
{
    [Serializable]
    [DataContract]
    public partial class DtoPaymentRequest
    {
        [DataMember]
        [Required(ErrorMessage = Constant.RequiredField)]
        public string PixKey { get; set; } = string.Empty;

        [DataMember]
        [Required(ErrorMessage = Constant.RequiredField)]
        [Range(0.01, 1000000.00, ErrorMessage = "{0} value must be greater than {1} and lower than {2}.")]
        public decimal Amount { get; set; } = 0m;
    }
}



using System.ComponentModel;

namespace LucasRT.DGBK.RestApi.Domain.ValuedObjects
{
    public static partial class Enums
    {
        public enum PaymentStatus
        {
            [Description("Criado")]
            Created,
            [Description("Capturado")]
            Captured,
            [Description("Parcialmente Reembolsado")]
            PartialRefunded,
            [Description("Totalmente Reembolsado")]
            FullRefunded,
            [Description("Reembolso falhou")]
            RefundFailed,
            [Description("Falhou")]
            Failed
        }

        public enum RefundStatus
        {
            [Description("Criado")]
            Created,
            [Description("Bem-sucedido")]
            Succeeded,
            [Description("Falhou")]
            Failed
        }
    }
}

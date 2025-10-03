

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
            [Description("Reembolsado")]
            Refunded,
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

        public enum WebhookStatus
        {
            [Description("Pendente")]
            Pending,
            [Description("Entregue")]
            Delivered,
            [Description("Falhou")]
            Failed
        }
    }
}

using LucasRT.DGBK.RestApi.Application.Services.Interfaces.Payments;
using LucasRT.DGBK.RestApi.Application.Services.Interfaces.Refunds;
using LucasRT.DGBK.RestApi.Application.Services.Payments;
using LucasRT.DGBK.RestApi.Application.Services.Refunds;

namespace LucasRT.DGBK.RestApi.Application.Services
{
    public static class DependencyInjection
    {
        public static void AddServices(this IServiceCollection service)
        {
            service.AddScoped<IPaymentService, PaymentService>();
            service.AddScoped<IRefundService, RefundService>();
        }
    }
}

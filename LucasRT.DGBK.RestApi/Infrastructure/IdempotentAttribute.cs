using LeadSoft.Common.Library.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json.Serialization;

namespace LucasRT.DGBK.RestApi.Infrastructure
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class IdempotentAttribute(int cacheTimeInMinutes = IdempotentAttribute.DefaultCacheTimeInMinutes) : Attribute, IAsyncActionFilter
    {
        private const int DefaultCacheTimeInMinutes = 60;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(cacheTimeInMinutes);

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string idempotenceKey = context.HttpContext.Request.GetHeader("IdempotencyKey");

            if (idempotenceKey.IsNothing() || idempotenceKey.ToGuid().IsNothing())
            {
                context.Result = new BadRequestObjectResult("Invalid or missing IdempotencyKey header");
                return;
            }

            IDistributedCache cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();

            string? cachedResult = await cache.GetStringAsync(idempotenceKey);
            if (cachedResult is not null)
            {
                IdempotentResponse response = cachedResult.JsonToObject<IdempotentResponse>();

                context.Result = new ObjectResult(response.Value) { StatusCode = StatusCodes.Status200OK };

                return;
            }

            ActionExecutedContext executedContext = await next();

            if (executedContext.Result is ObjectResult { StatusCode: >= 200 and < 300 } objectResult)
            {
                int statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;
                IdempotentResponse response = new(statusCode, objectResult.Value);

                await cache.SetStringAsync(idempotenceKey, response.ToJson(), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _cacheDuration });
            }
        }
    }

    [method: JsonConstructor]
    public sealed class IdempotentResponse(int statusCode, dynamic value)
    {
        public int StatusCode { get; } = statusCode;
        public dynamic Value { get; } = value;
    }
}

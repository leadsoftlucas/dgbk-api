using LucasRT.DGBK.RestApi.Application.Services;
using LucasRT.DGBK.RestApi.Configurations;
using LucasRT.DGBK.RestApi.Workers;
using Swashbuckle.AspNetCore.SwaggerUI;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddPostgreSQL(builder.Configuration);

builder.Services.AddControllersConfig();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddResponseCompression();
builder.Services.AddSwaggerConfig();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddServices();

builder.Services.AddHostedService<PaymentWebhookDeliveryWorker>();
builder.Services.AddHostedService<RefundWebhookDeliveryWorker>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", $"RavenDB Sales Assistant Demo");
        c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
        c.DocExpansion(DocExpansion.None);
        c.DefaultModelExpandDepth(2);
        c.DefaultModelRendering(ModelRendering.Example);
        c.DisplayOperationId();
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableTryItOutByDefault();
        c.EnableValidator();
        c.ShowCommonExtensions();
    });
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapControllers();

app.Run();

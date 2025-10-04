using LeadSoft.Common.Library.EnvUtils;
using LeadSoft.Common.Library.Extensions;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace LucasRT.DGBK.RestApi.Configurations
{
    /// <summary>
    /// Provides extension methods for configuring Swagger in an ASP.NET Core application.
    /// </summary>
    /// <remarks>This class contains methods to integrate Swagger documentation generation into the
    /// application's service collection. It enables features such as XML comments inclusion, custom schema IDs, and
    /// annotations.</remarks>
    public static class SwaggerConfiguration
    {
        /// <summary>
        /// Configures Swagger services for the application, enabling API documentation generation.
        /// </summary>
        /// <remarks>This method adds Swagger generation capabilities to the application's service
        /// collection. It registers the Swagger documentation with metadata such as title, version, description, and
        /// contact information. Additionally, it includes XML comments for API documentation, enables annotations, and
        /// customizes schema IDs.</remarks>
        /// <param name="services">The <see cref="IServiceCollection"/> to which Swagger services will be added.</param>
        public static void AddSwaggerConfig(this IServiceCollection services)
        {
            services.AddSwaggerGen(
                 c =>
                 {
                     c.SwaggerDoc("v1", new OpenApiInfo
                     {
                         Title = $"DGBK Rest Api - {EnvUtil.Get(EnvUtil.AspNet)} - RavenDB",
                         Version = "v1",
                         Description = Assembly.GetExecutingAssembly().GetEmbeddedResourceContent($"README.md", true),
                         Contact = new OpenApiContact
                         {
                             Name = "Lucas Tavares",
                             Email = "lucas.tavares@ravendb.net"
                         }
                     });

                     c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{PlatformServices.Default.Application.ApplicationName}.xml"));
                     c.EnableAnnotations();
                     c.CustomSchemaIds(s => s.FullName.Replace("+", "."));
                 }
             );
        }
    }
}

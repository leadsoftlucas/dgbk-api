using LeadSoft.Common.Library.EnvUtils;
using LeadSoft.Common.Library.Extensions;
using LucasRT.DGBK.RestApi.Domain;
using Raven.DependencyInjection;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace LucasRT.DGBK.RestApi.Configurations
{
    public static partial class RavenDBConfiguration
    {
        private static X509Certificate2 GetRavenDBCertificate(IConfiguration configuration)
        {
            X509Certificate2? cert = null;

            if (configuration["RavenSettings:CertificateResourceName"].IsSomething())
                cert = Assembly.GetExecutingAssembly()
                               .GetEmbeddedX509Certificate($"{configuration["RavenSettings:CertificateResourceName"]}", EnvUtil.Get(EnvConstant.RavenDBPwd));


            return cert ?? throw new OperationCanceledException("Certificate not found!");
        }

        public static void AddRavenDB(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddRavenDbDocStore(options =>
            {
                options.Certificate = GetRavenDBCertificate(configuration);
            });
            service.AddRavenDbAsyncSession();
        }
    }
}

using LeadSoft.Common.Library.EnvUtils;
using LeadSoft.Common.Library.Extensions;
using LucasRT.DGBK.RestApi.Domain;
using Newtonsoft.Json;
using Raven.Client.Json.Serialization.NewtonsoftJson;
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
                options.BeforeInitializeDocStore = docStore =>
                {
                    docStore.Conventions.Serialization = new NewtonsoftJsonSerializationConventions
                    {
                        CustomizeJsonSerializer = serializer => serializer.NullValueHandling = NullValueHandling.Ignore,
                    };
                    //docStore.Maintenance.Send(new ConfigureRefreshOperation(new RefreshConfiguration
                    //{
                    //    Disabled = false,
                    //    RefreshFrequencyInSec = 2,
                    //    MaxItemsToProcess = 1000
                    //}));
                };
            });
            service.AddRavenDbAsyncSession();
        }
    }
}

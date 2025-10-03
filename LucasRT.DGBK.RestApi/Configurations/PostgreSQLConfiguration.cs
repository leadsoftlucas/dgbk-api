using LucasRT.DGBK.RestApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LucasRT.DGBK.RestApi.Configurations
{
    public static class PostgreSQLConfiguration
    {
        public static void AddPostgreSQL(this IServiceCollection service, IConfiguration _Configuration)
        {
            service.AddDbContext<PostgreSQL>(
                options => options.UseNpgsql(_Configuration.GetConnectionString("PostgreSQL"))
                );
            service.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}

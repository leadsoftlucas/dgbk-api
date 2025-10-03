using LucasRT.DGBK.RestApi.Domain.Entities.Payments;
using LucasRT.DGBK.RestApi.Domain.Entities.Refunds;
using LucasRT.DGBK.RestApi.Infrastructure.Data.Persistence.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LucasRT.DGBK.RestApi.Infrastructure.Data
{
    public class PostgreSQL : DbContext
    {
        private readonly IConfiguration _Configuration;

        public PostgreSQL()
        {
        }

        public PostgreSQL(DbContextOptions<PostgreSQL> options) : base(options)
        {
        }

        public PostgreSQL(IConfiguration configuration) : base()
        {
            _Configuration = configuration;
        }

        public PostgreSQL(IConfiguration configuration, DbContextOptions<PostgreSQL> options) : base(options)
        {
            _Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(_Configuration.GetConnectionString("PostgreSQL"));
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostgreSQL).Assembly);

            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetProperties()
                    .Where(p => p.ClrType.IsEnum)))
                property.SetColumnType("varchar(50)");

            modelBuilder.ApplyConfiguration(new PaymentsConfig());
            modelBuilder.ApplyConfiguration(new PaymentStatusHistoryConfig());
            modelBuilder.ApplyConfiguration(new RefundsConfig());

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentStatusHistory> PaymentStatusHistories { get; set; }
        public DbSet<Refund> Refunds { get; set; }
    }

    public class PostgreSQLDesignTimeFactory : IDesignTimeDbContextFactory<PostgreSQL>
    {
        public PostgreSQL CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory());

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.Development.json", optional: false)
                .Build();

            string connectionString = configuration.GetConnectionString("PostgreSQL");

            DbContextOptionsBuilder<PostgreSQL> optionsBuilder = new();
            optionsBuilder.UseNpgsql(connectionString);

            return new PostgreSQL(optionsBuilder.Options);
        }
    }
}

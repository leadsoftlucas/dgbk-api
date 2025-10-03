using LucasRT.DGBK.RestApi.Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LucasRT.DGBK.RestApi.Infrastructure.Data.Persistence.Payments
{
    public class PaymentStatusHistoryConfig : IEntityTypeConfiguration<PaymentStatusHistory>
    {
        public void Configure(EntityTypeBuilder<PaymentStatusHistory> b)
        {
            b.HasKey(h => h.Id);

            b.Property(h => h.Status)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            b.Property(h => h.Reason).HasMaxLength(512);
            b.Property(h => h.At).IsRequired();

            b.HasIndex(h => new { h.PaymentId, h.At });
        }
    }
}
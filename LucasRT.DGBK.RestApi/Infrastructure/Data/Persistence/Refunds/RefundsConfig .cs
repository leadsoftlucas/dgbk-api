using LucasRT.DGBK.RestApi.Domain.Entities.Refunds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LucasRT.DGBK.RestApi.Infrastructure.Data.Persistence.Payments
{
    public class RefundsConfig : IEntityTypeConfiguration<Refund>
    {
        public void Configure(EntityTypeBuilder<Refund> b)
        {
            b.HasKey(r => r.Id);

            b.Property(r => r.Status)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            b.Property(r => r.CreatedAt).IsRequired();
            b.Property(r => r.CompletedAt);

            b.Property(p => p.Amount)
               .HasPrecision(18, 2);

            b.HasOne(r => r.Payment)
             .WithMany()
             .HasForeignKey(r => r.PaymentId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(r => new { r.PaymentId, r.Status, r.CreatedAt });
        }
    }
}
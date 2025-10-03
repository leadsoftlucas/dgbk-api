using LucasRT.DGBK.RestApi.Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LucasRT.DGBK.RestApi.Infrastructure.Data.Persistence.Payments
{
    public class PaymentsConfig : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> b)
        {
            b.HasKey(p => p.Id);

            b.Property(p => p.CreatedAt);
            b.Property(p => p.CapturedAt);

            b.Property(p => p.Amount)
                .HasPrecision(18, 2);

            b.Property(p => p.RefundedAmount)
                .HasPrecision(18, 2);

            b.Property(p => p.PixKey)
                .HasMaxLength(140);

            b.HasMany<PaymentStatusHistory>()
             .WithOne(h => h.Payment!)
             .HasForeignKey(h => h.PaymentId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(p => p.Status);
            b.HasIndex(p => p.CreatedAt);
        }
    }
}
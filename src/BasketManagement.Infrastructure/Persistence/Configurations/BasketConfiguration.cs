using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BasketManagement.Domain.Entities;

namespace BasketManagement.Infrastructure.Persistence.Configurations;

public class BasketConfiguration : IEntityTypeConfiguration<Basket>
{
    public void Configure(EntityTypeBuilder<Basket> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.UserId).IsRequired();
        builder.Property(b => b.Status).HasConversion<int>().IsRequired();
        builder.Property(b => b.CreatedAt).IsRequired();
        builder.Property(b => b.LastUpdatedAt).IsRequired(false);

        builder.HasMany(b => b.Items)
            .WithOne(i => i.Basket)
            .HasForeignKey(i => i.BasketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(b => b.DomainEvents);
    }
}
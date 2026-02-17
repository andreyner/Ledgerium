
using Ledgerium.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ledgerium.Infrastructure.Persistence.Configurations;

public sealed class BalanceConfiguration : IEntityTypeConfiguration<BalanceEntity>
{
    public void Configure(EntityTypeBuilder<BalanceEntity> b)
    {
        b.ToTable("balances");

        b.HasKey(x => x.ClientId);

        b.Property(x => x.Value)
            .HasPrecision(18, 2)
            .IsRequired();
    }
}
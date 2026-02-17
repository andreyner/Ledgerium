using Ledgerium.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ledgerium.Infrastructure.Persistence.Configurations;

public sealed class TransactionEntityConfiguration : IEntityTypeConfiguration<TransactionEntity>
{
    public void Configure(EntityTypeBuilder<TransactionEntity> b)
    {
        b.ToTable("transactions");
        
        b.HasKey(x => x.Id);
        
        b.HasIndex(x => x.Id).IsUnique();
        
        b.Property(x => x.Type)
            .IsRequired()
            .HasConversion<int>();

        b.Property(x => x.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        b.Property(x => x.OccurredAt)
            .IsRequired();

        b.Property(x => x.InsertedAt)
            .IsRequired();

        b.Property(x => x.IsReverted)
            .IsRequired();

        b.Property(x => x.RevertedAt)
            .IsRequired(false);
        
        b.HasOne<BalanceEntity>()
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
using Ledgerium.Infrastructure.Entities;
using Ledgerium.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
namespace Ledgerium.Infrastructure.Persistence;

public sealed class WalletDbContext : DbContext
{
    public DbSet<TransactionEntity> Transactions => Set<TransactionEntity>();
    public DbSet<BalanceEntity> Balances => Set<BalanceEntity>();

    public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TransactionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new BalanceConfiguration());
    }
}
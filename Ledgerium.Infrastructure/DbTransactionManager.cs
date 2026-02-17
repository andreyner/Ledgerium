using System.Data;
using Ledgerium.Application.Services;
using Microsoft.EntityFrameworkCore;
using Ledgerium.Infrastructure.Persistence;

namespace Ledgerium.Infrastructure;

public sealed class DbTransactionManager: IDbTransactionManager
{
    private readonly WalletDbContext _db;

    public DbTransactionManager(WalletDbContext db)
    {
        _db = db;
    }

    public async Task<IAppDbTransaction> BeginAsync(CancellationToken ct)
    {
        var tx = await _db.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, ct);

        return new AppDbTransaction(tx);
    }
}
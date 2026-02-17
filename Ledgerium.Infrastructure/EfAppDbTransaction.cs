using Ledgerium.Application.Services;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ledgerium.Infrastructure;

internal sealed class AppDbTransaction : IAppDbTransaction
{
    private readonly IDbContextTransaction _tx;

    public AppDbTransaction(IDbContextTransaction tx)
    {
        _tx = tx;
    }

    public Task CommitAsync(CancellationToken ct)
        => _tx.CommitAsync(ct);

    public Task RollbackAsync(CancellationToken ct)
        => _tx.RollbackAsync(ct);

    public ValueTask DisposeAsync()
        => _tx.DisposeAsync();
}
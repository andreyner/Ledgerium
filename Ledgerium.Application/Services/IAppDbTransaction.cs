namespace Ledgerium.Application.Services;

public interface IAppDbTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken ct);
    Task RollbackAsync(CancellationToken ct);
}
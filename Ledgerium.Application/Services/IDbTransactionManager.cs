namespace Ledgerium.Application.Services;

public interface IDbTransactionManager
{
    Task<IAppDbTransaction> BeginAsync(CancellationToken ct);
}
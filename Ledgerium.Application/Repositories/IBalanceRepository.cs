using Ledgerium.Domain;

namespace Ledgerium.Application.Repositories;

public interface IBalanceRepository
{
    Task<Balance?> Get(Guid clientId, CancellationToken ct);
    Task<Balance> CreateBalance(Guid clientId, CancellationToken ct);
    Task Save(Balance balance, CancellationToken ct);
}
using Ledgerium.Domain.Abstractions;

namespace Ledgerium.Application.Repositories;

public interface ITransactionRepository
{
    Task<Transaction?> GetById(Guid id, CancellationToken ct);
    Task Add(Transaction tx, CancellationToken ct);
    Task Save(Transaction transaction, CancellationToken ct);
}
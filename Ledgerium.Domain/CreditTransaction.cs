using Ledgerium.Domain.Abstractions;

namespace Ledgerium.Domain;

public sealed class CreditTransaction : Transaction
{
    public CreditTransaction(Guid id, Guid clientId, DateTimeOffset occurredAt, decimal amount, DateTimeOffset insertedAt, bool isReverted, DateTimeOffset? revertedAt)
        : base(id, clientId, occurredAt, amount, insertedAt, isReverted, revertedAt) { }

    public override TransactionType Type => TransactionType.Credit;
}
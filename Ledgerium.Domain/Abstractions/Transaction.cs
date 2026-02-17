using Ledgerium.Domain.Exceptions;

namespace Ledgerium.Domain.Abstractions;

public abstract class Transaction
{
    public Guid Id { get; }
    public Guid ClientId { get; }
    public decimal Amount { get; }
    public DateTimeOffset OccurredAt { get; }
    public DateTimeOffset InsertedAt { get; }
    public bool IsReverted { get; private set; }
    public DateTimeOffset? RevertedAt { get; private set; }
    public abstract TransactionType Type { get; }

    protected Transaction(Guid id, Guid clientId, DateTimeOffset occurredAt, decimal amount, DateTimeOffset insertedAt, bool isReverted, DateTimeOffset? revertedAt)
    {
        if (id == Guid.Empty)
            throw new DomainException("Transaction id is required");

        if (clientId == Guid.Empty)
            throw new DomainException("Client id is required");

        if (amount <= 0)
            throw new DomainException("Amount must be positive");

        if (occurredAt > DateTime.UtcNow)
            throw new DomainException("Transaction date cannot be in the future");

        Id = id;
        ClientId = clientId;
        OccurredAt = occurredAt;
        Amount = amount;
        InsertedAt = insertedAt;
        IsReverted = isReverted;
        RevertedAt = revertedAt;
    }
    public void Revert()
    {
        if (IsReverted)
            throw new DomainException("Transaction already reverted");

        IsReverted = true;
        RevertedAt = DateTimeOffset.UtcNow;
    }
}
using Ledgerium.Domain.Exceptions;

namespace Ledgerium.Domain;

public sealed class Balance
{
    public Guid ClientId { get; private set; }
    public decimal Value { get; private set; }
    
    public Balance(Guid clientId, decimal initialValue = 0m)
    {
        if (clientId == Guid.Empty)
            throw new ArgumentException("ClientId cannot be empty", nameof(clientId));

        if (initialValue < 0)
            throw new ArgumentOutOfRangeException(nameof(initialValue), "Initial balance cannot be negative");

        ClientId = clientId;
        Value = initialValue;
    }

    public void Credit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Credit amount must be positive");

        Value += amount;
    }

    public void Debit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Debit amount must be positive");

        if (Value < amount)
            throw new InsufficientFundsException(ClientId, Value, amount);

        Value -= amount;
    }

    public void ApplyRevert(decimal amount, TransactionType type)
    {
        if (amount <= 0)
            throw new ArgumentException("Revert amount must be positive");

        switch (type)
        {
            case TransactionType.Credit:
                // откат зачисления = списать
                if (Value < amount)
                    throw new DomainException("Cannot revert credit: insufficient funds");
                Value -= amount;
                break;

            case TransactionType.Debit:
                // откат списания = зачислить
                Value += amount;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown transaction type");
        }
    }
}
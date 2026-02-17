namespace Ledgerium.Domain.Exceptions;

/// <summary>
/// Исключение возникает, когда клиент пытается списать больше средств, чем есть на балансе
/// </summary>
public sealed class InsufficientFundsException : DomainException
{
    /// <summary>
    /// Идентификатор клиента
    /// </summary>
    public Guid ClientId { get; }

    /// <summary>
    /// Текущий баланс клиента
    /// </summary>
    public decimal CurrentBalance { get; }

    /// <summary>
    /// Запрошенная сумма списания
    /// </summary>
    public decimal RequestedAmount { get; }

    public InsufficientFundsException(Guid clientId, decimal currentBalance, decimal requestedAmount)
        : base($"Insufficient funds for client {clientId}. Current balance: {currentBalance}, requested: {requestedAmount}.")
    {
        ClientId = clientId;
        CurrentBalance = currentBalance;
        RequestedAmount = requestedAmount;
    }
}
namespace Ledgerium.Application.Dto;

public sealed class RevertResult
{
    /// <summary>
    /// Дата и время выполнения отмены транзакции
    /// </summary>
    public DateTimeOffset RevertDateTime { get; }

    /// <summary>
    /// Баланс клиента после отмены транзакции
    /// </summary>
    public decimal ClientBalance { get; }

    public RevertResult(DateTimeOffset revertDateTime, decimal clientBalance)
    {
        RevertDateTime = revertDateTime;
        ClientBalance = clientBalance;
    }
}
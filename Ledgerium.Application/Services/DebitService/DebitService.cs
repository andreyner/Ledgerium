using Ledgerium.Application.Commands;
using Ledgerium.Application.Dto;
using Ledgerium.Application.Repositories;
using Ledgerium.Domain;

namespace Ledgerium.Application.Services.DebitService;

public class DebitService: IDebitService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IBalanceRepository _balanceRepository;
    private readonly IDbTransactionManager _dbTransactionManager;

    public DebitService(
        ITransactionRepository transactionRepository,
        IBalanceRepository balanceRepository, 
        IDbTransactionManager dbTransactionManager)
    {
        _transactionRepository = transactionRepository;
        _balanceRepository = balanceRepository;
        _dbTransactionManager = dbTransactionManager;
    }
    public async Task<DebitResult> DebitAsync(DebitCommand command, CancellationToken ct)
    {
        // 1. Транзакция с максимальной изоляцией
        await using var tx = await _dbTransactionManager.BeginAsync(ct);
        try
        {
            // 2. Идемпотентность
            var existing = await _transactionRepository.GetById(command.Id, ct);
            if (existing != null)
            {
                return new DebitResult(
                    existing.OccurredAt,
                    await GetCurrentBalance(command.ClientId, ct));
            }

            // 3. Получаем баланс
            var balance = await _balanceRepository.Get(command.ClientId, ct);
            
            if (balance == null)
            {
                balance = await _balanceRepository.CreateBalance(command.ClientId, ct);
            }

            // 4. Доменная транзакция
            var transaction = new DebitTransaction(
                command.Id,
                command.ClientId,
                command.DateTime,
                command.Amount,
                DateTime.UtcNow,
                false,
                null);

            // 5. Применяем доменную логику (может кинуть InsufficientFundsException)
            balance.Debit(transaction.Amount);

            await _transactionRepository.Add(transaction, ct);
            await _balanceRepository.Save(balance, ct);
        
            await  tx.CommitAsync(ct);
            
            return new DebitResult(transaction.OccurredAt, balance.Value);
        }
        catch (Exception)
        {
            await  tx.RollbackAsync(ct);
            throw;
        }
    }

    private async Task<decimal> GetCurrentBalance(Guid clientId, CancellationToken ct)
    {
        var balance = await _balanceRepository.Get(clientId, ct);
        return balance?.Value ?? 0m;
    }
}
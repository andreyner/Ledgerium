using System.ComponentModel.DataAnnotations;
using Ledgerium.Application.Commands;
using Ledgerium.Application.Dto;
using Ledgerium.Application.Repositories;
using Ledgerium.Domain.Exceptions;

namespace Ledgerium.Application.Services.RevertService;

public class RevertService: IRevertService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IBalanceRepository _balanceRepository;
    private readonly IDbTransactionManager _dbTransactionManager;

    public RevertService(IBalanceRepository balanceRepository,
        ITransactionRepository transactionRepository, 
        IDbTransactionManager dbTransactionManager)
    {
        _balanceRepository = balanceRepository;
        _transactionRepository = transactionRepository;
        _dbTransactionManager = dbTransactionManager;
    }


    public async Task<RevertResult> RevertAsync(RevertCommand command, CancellationToken ct)
    {
        if (command.TransactionId == Guid.Empty)
            throw new ValidationException("TransactionId cannot be empty");

        await using var tx = await _dbTransactionManager.BeginAsync(ct);
        try
        {
            // 1. Находим транзакцию
            var transaction = await _transactionRepository.GetById(command.TransactionId, ct);
            if (transaction == null)
                throw new NotFoundException($"Transaction {command.TransactionId} not found");

            // 2. Идемпотентность: повторный revert
            if (transaction.IsReverted)
            {
                var currentBalance = await GetCurrentBalance(transaction.ClientId, ct);
                return new RevertResult(transaction.RevertedAt!.Value, currentBalance);
            }

            // 3. Загружаем баланс
            var balance = await _balanceRepository.Get(transaction.ClientId, ct);

            // 4. Применяем откат
            balance.ApplyRevert(transaction.Amount, transaction.Type);
            
            // 5. Обновляем флаги в БД
            transaction.Revert();
            
            await _transactionRepository.Save(transaction, ct);
            await _balanceRepository.Save(balance, ct);

            await tx.CommitAsync(ct);
            
            return new RevertResult(transaction.RevertedAt.Value, balance.Value);
        }
        catch (Exception)
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    private async Task<decimal> GetCurrentBalance(Guid clientId, CancellationToken ct)
    {
        var balance = await _balanceRepository.Get(clientId, ct);
        return balance?.Value ?? 0m;
    }
}
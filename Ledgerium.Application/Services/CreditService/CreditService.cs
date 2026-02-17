using Ledgerium.Application.Commands;
using Ledgerium.Application.Dto;
using Ledgerium.Application.Repositories;
using Ledgerium.Domain;

namespace Ledgerium.Application.Services.CreditService;

public sealed class CreditService : ICreditService
{
    private readonly IDbTransactionManager _dbTransactionManager;
    private readonly ITransactionRepository _transactionsRepository;
    private readonly IBalanceRepository _balanceRepository;

    public CreditService(
        IDbTransactionManager dbTransactionManager,
        ITransactionRepository transactionsRepository,
        IBalanceRepository balanceRepository)
    {
        _dbTransactionManager = dbTransactionManager;
        _transactionsRepository = transactionsRepository;
        _balanceRepository = balanceRepository;
    }

    public async Task<CreditResult> CreditAsync(CreditCommand command, CancellationToken ct)
    {
        // 1. Транзакция с максимальной изоляцией
        await using var tx = await _dbTransactionManager.BeginAsync(ct);
        try
        {
            // 2. Идемпотентность
            var existingTx = await _transactionsRepository.GetById(command.Id, ct);
            if (existingTx != null)
                return new CreditResult(existingTx.InsertedAt, existingTx.Amount);
            
            var transaction = new CreditTransaction(
                command.Id,
                command.ClientId,
                command.DateTime,
                command.Amount,
                DateTime.UtcNow,
                false,
                null);
            
            var balance = await _balanceRepository.Get(command.ClientId, ct);
            
            if (balance == null)
            {
                balance = await _balanceRepository.CreateBalance(command.ClientId, ct);
            }

            balance.Credit(command.Amount);
            
            await _transactionsRepository.Add(transaction, ct);
            await _balanceRepository.Save(balance, ct);
            await tx.CommitAsync(ct);

            return new CreditResult(transaction.InsertedAt, balance.Value);
        }
        catch(Exception)
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}

using AutoMapper;
using Ledgerium.Application.Repositories;
using Ledgerium.Domain.Abstractions;
using Ledgerium.Infrastructure.Entities;
using Ledgerium.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ledgerium.Infrastructure.Repositories;

public sealed class TransactionRepository : ITransactionRepository
{
    private readonly WalletDbContext _db;
    private readonly IMapper _mapper;

    public TransactionRepository(WalletDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Transaction?> GetById(Guid id, CancellationToken ct)
    {
        var transactionEntity = await _db.Transactions.FirstOrDefaultAsync(x => x.Id == id, ct);
        
        return _mapper.Map<Transaction>(transactionEntity);
    }

    public async Task Add(Transaction tx, CancellationToken ct)
    {
        var transactionEntity = _mapper.Map<TransactionEntity>(tx);
        await _db.Transactions.AddAsync(transactionEntity, ct);
    }

    public async Task Save(Transaction transaction, CancellationToken ct)
    {
        var transactionEntity = await _db.Transactions
            .FirstOrDefaultAsync(x => x.Id == transaction.Id, ct);

        if (transactionEntity == null)
            throw new InvalidOperationException($"Transaction {transaction.Id} not found");

        _mapper.Map(transaction, transactionEntity); // маппинг Domain → Entity

        await _db.SaveChangesAsync(ct);
    }
}
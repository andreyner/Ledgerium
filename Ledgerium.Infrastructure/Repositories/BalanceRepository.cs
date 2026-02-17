using AutoMapper;
using Ledgerium.Application.Repositories;
using Ledgerium.Domain;
using Ledgerium.Infrastructure.Entities;
using Ledgerium.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ledgerium.Infrastructure.Repositories;

public sealed class BalanceRepository : IBalanceRepository
{
    private readonly WalletDbContext _db;
    private readonly IMapper _mapper;

    public BalanceRepository(WalletDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Balance?> Get(Guid clientId, CancellationToken ct)
    { 
         var balanceEntity = await _db.Balances.AsNoTracking().FirstOrDefaultAsync(x => x.ClientId == clientId, ct);
         return balanceEntity == null ? null : _mapper.Map<Balance>(balanceEntity);
    }
    
    public async Task<Balance> CreateBalance(Guid clientId, CancellationToken ct)
    {
        var entity = new BalanceEntity { ClientId = clientId, Value = 0 };
        _db.Balances.Add(entity);
        await _db.SaveChangesAsync(ct);

        return _mapper.Map<Balance>(entity);
    }

    public async Task Save(Balance balance, CancellationToken ct)
    {
        var balanceEntity = await _db.Balances
            .FirstOrDefaultAsync(x => x.ClientId == balance.ClientId, ct);
        
        if (balanceEntity == null)
        {
            balanceEntity = _mapper.Map<BalanceEntity>(balance);
            _db.Balances.Add(balanceEntity);
        }
        
        _mapper.Map(balance, balanceEntity);
        
        await _db.SaveChangesAsync(ct);
    }
}
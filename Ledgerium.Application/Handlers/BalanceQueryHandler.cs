using System.ComponentModel.DataAnnotations;
using Ledgerium.Application.Abstractions;
using Ledgerium.Application.Dto;
using Ledgerium.Application.Queries;
using Ledgerium.Application.Repositories;

namespace Ledgerium.Application.Handlers;

public sealed class BalanceQueryHandler 
    : IQueryHandler<GetBalanceQuery, BalanceDto>
{
    private readonly IBalanceRepository _balances;

    public BalanceQueryHandler(IBalanceRepository balances)
    {
        _balances = balances;
    }

    public async Task<BalanceDto> Handle(GetBalanceQuery query, CancellationToken ct)
    {
        if (query.ClientId == Guid.Empty)
            throw new ValidationException("ClientId cannot be empty");

        var balance = await _balances.Get(query.ClientId, ct);

        return new BalanceDto
        {
            BalanceDateTime = DateTimeOffset.UtcNow,
            ClientBalance = balance?.Value ?? 0m
        };
    }
}
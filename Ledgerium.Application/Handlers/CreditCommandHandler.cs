using Ledgerium.Application.Abstractions;
using Ledgerium.Application.Commands;
using Ledgerium.Application.Dto;
using Ledgerium.Application.Services;

namespace Ledgerium.Application.Handlers;

public sealed class CreditCommandHandler 
    : ICommandHandler<CreditCommand, CreditResult>
{
    private readonly ICreditService _creditService;

    public CreditCommandHandler(
        ICreditService creditService)
    {
        _creditService = creditService;
    }

    public async Task<CreditResult> Handle(CreditCommand command, CancellationToken ct)
    {
        return await _creditService.CreditAsync(command, ct);
    }
}

using Ledgerium.Application.Abstractions;
using Ledgerium.Application.Commands;
using Ledgerium.Application.Dto;
using Ledgerium.Application.Services.DebitService;

namespace Ledgerium.Application.Handlers;

public sealed class DebitCommandHandler 
    : ICommandHandler<DebitCommand, DebitResult>
{
    private readonly IDebitService _debitService;

    public DebitCommandHandler(IDebitService debitService)
    {
        _debitService = debitService;
    }

    public async Task<DebitResult> Handle(DebitCommand cmd, CancellationToken ct)
    {
        return await _debitService.DebitAsync(cmd, ct);
    }
}
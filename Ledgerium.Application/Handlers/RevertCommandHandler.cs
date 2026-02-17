using Ledgerium.Application.Abstractions;
using Ledgerium.Application.Commands;
using Ledgerium.Application.Dto;
using Ledgerium.Application.Services.RevertService;

namespace Ledgerium.Application.Handlers;

public sealed class RevertCommandHandler 
    : ICommandHandler<RevertCommand, RevertResult>
{
    private readonly IRevertService _revertService;

    public RevertCommandHandler(IRevertService revertService)
    {
        _revertService = revertService;
    }

    public async Task<RevertResult> Handle(RevertCommand cmd, CancellationToken ct)
    {
        return await _revertService.RevertAsync(cmd, ct);
    }
    
}
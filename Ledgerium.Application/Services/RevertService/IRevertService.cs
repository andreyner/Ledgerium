using Ledgerium.Application.Commands;
using Ledgerium.Application.Dto;

namespace Ledgerium.Application.Services.RevertService;

public interface IRevertService
{
    Task<RevertResult> RevertAsync(RevertCommand command, CancellationToken ct);
}
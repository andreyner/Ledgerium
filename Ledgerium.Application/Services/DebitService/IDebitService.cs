using Ledgerium.Application.Commands;
using Ledgerium.Application.Dto;

namespace Ledgerium.Application.Services.DebitService;

public interface IDebitService
{
    Task<DebitResult> DebitAsync(DebitCommand command, CancellationToken ct);
}
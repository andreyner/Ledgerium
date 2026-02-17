using Ledgerium.Application.Commands;
using Ledgerium.Application.Dto;

namespace Ledgerium.Application.Services;

public interface ICreditService
{
    Task<CreditResult> CreditAsync(CreditCommand command, CancellationToken ct);
}
using Ledgerium.Application.Abstractions;
using Ledgerium.Application.Commands;
using Ledgerium.Application.Dto;
using Ledgerium.Application.Queries;
using Ledgerium.Web.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Ledgerium.Web.Controllers;

[ApiController]
[Route("")]
public sealed class TransactionsController : ControllerBase
{
    [HttpPost("credit")]
    [ProducesResponseType(typeof(CreditResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Credit(
        [FromBody] CreditRequest request,
        [FromServices] ICommandHandler<CreditCommand, CreditResult> handler,
        CancellationToken ct)
    {
        var command = new CreditCommand(
            request.Id,
            request.ClientId,
            request.DateTime,
            request.Amount);

        var result = await handler.Handle(command, ct);

        return Ok(result);
    }

    [HttpPost("debit")]
    [ProducesResponseType(typeof(DebitResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Debit(
        [FromBody] DebitRequest request,
        [FromServices] ICommandHandler<DebitCommand, DebitResult> handler,
        CancellationToken ct)
    {
        var command = new DebitCommand(
            request.Id,
            request.ClientId,
            request.DateTime,
            request.Amount);

        var result = await handler.Handle(command, ct);

        return Ok(result);
    }

    [HttpPost("revert")]
    [ProducesResponseType(typeof(RevertResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Revert(
        [FromQuery] Guid id,
        [FromServices] ICommandHandler<RevertCommand, RevertResult> handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(new RevertCommand(id), ct);

        return Ok(result);
    }

    [HttpGet("balance")]
    [ProducesResponseType(typeof(BalanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Balance(
        [FromQuery] Guid id,
        [FromServices] IQueryHandler<GetBalanceQuery, BalanceDto> handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(new GetBalanceQuery(id), ct);

        return Ok(result);
    }
}
namespace Ledgerium.Application.Commands;

public sealed record DebitCommand(
    Guid Id,
    Guid ClientId,
    DateTimeOffset DateTime,
    decimal Amount
);
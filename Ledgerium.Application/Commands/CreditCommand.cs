namespace Ledgerium.Application.Commands;

public record CreditCommand(
    Guid Id,
    Guid ClientId,
    DateTimeOffset DateTime,
    decimal Amount
);
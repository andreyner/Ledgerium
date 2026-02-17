namespace Ledgerium.Application.Dto;

public sealed record DebitResult(
    DateTimeOffset InsertDateTime,
    decimal ClientBalance
    );
namespace Ledgerium.Application.Dto;

public sealed record CreditResult(
    DateTimeOffset InsertDateTime,
    decimal ClientBalance
    );
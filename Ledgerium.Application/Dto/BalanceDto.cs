namespace Ledgerium.Application.Dto;

public sealed record BalanceDto
{
    public DateTimeOffset BalanceDateTime { get; init; }
    public decimal ClientBalance { get; init; }
}
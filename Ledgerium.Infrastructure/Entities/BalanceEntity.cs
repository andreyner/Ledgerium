namespace Ledgerium.Infrastructure.Entities;

public class BalanceEntity
{
    public Guid ClientId { get; set; }
    public decimal Value { get; set; }
}
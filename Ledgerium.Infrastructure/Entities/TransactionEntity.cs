namespace Ledgerium.Infrastructure.Entities;

public sealed class TransactionEntity
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public decimal Amount { get; set; }
    public int Type { get; set; } 
    public DateTimeOffset InsertedAt { get; set; }
    public bool IsReverted { get; set; }
    public DateTimeOffset? RevertedAt { get; set; }
}
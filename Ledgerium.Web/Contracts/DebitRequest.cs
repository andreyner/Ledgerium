using System.ComponentModel.DataAnnotations;
using Ledgerium.Web.Attributes;

namespace Ledgerium.Web.Contracts;

public sealed class DebitRequest
{
    [Required]
    public Guid Id { get; init; }

    [Required]
    public Guid ClientId { get; init; }

    [Required]
    public DateTimeOffset DateTime { get; init; }

    [PositiveDecimal]
    public decimal Amount { get; init; }
}
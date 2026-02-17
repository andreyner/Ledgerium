using System.ComponentModel.DataAnnotations;

namespace Ledgerium.Web.Attributes;

public sealed class PositiveDecimalAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
        => value is decimal d && d > 0;
}
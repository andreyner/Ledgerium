namespace Ledgerium.Domain.Exceptions;

/// <summary>
/// Базовое исключение для доменной логики
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
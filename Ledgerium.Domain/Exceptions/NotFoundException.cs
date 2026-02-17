namespace Ledgerium.Domain.Exceptions;

public sealed class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entity, object key)
        : base($"{entity} with key '{key}' was not found")
    {
    }
}
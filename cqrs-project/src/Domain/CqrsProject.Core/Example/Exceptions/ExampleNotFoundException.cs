using CqrsProject.Common.Exceptions;

namespace CqrsProject.Core.Exceptions;

public class ExampleNotFoundException : BusinessException
{
    public ExampleNotFoundException(string message) : base(message)
    {
    }

    public ExampleNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

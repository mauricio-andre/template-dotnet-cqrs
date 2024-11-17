using CqrsProject.Common.Exceptions;

namespace CqrsProject.Core.Exceptions;

public class DuplicatedExampleException : BusinessException
{
    public DuplicatedExampleException(string message) : base(message)
    {
    }

    public DuplicatedExampleException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

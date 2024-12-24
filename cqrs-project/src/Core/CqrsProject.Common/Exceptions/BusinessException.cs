using System.Diagnostics.CodeAnalysis;

namespace CqrsProject.Common.Exceptions;

[Serializable]
[SuppressMessage("Maintainability", "S3925", Justification = "A sobrecarga com SerializationInfo está obsoleta")]
public abstract class BusinessException : Exception
{
    public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();

    protected BusinessException(string message) : base(message)
    {
    }

    protected BusinessException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected BusinessException(string message, IDictionary<string, string[]> errors) : base(message)
    {
        Errors = errors;
    }
}

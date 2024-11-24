using CqrsProject.Common.Exceptions;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Exceptions;

public class ExampleNotFoundException : BusinessException
{
    public ExampleNotFoundException(IStringLocalizer localizer) : base(localizer["message:validation:exampleNotFound"])
    {
    }
}

using CqrsProject.Common.Exceptions;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Exceptions;

public class DuplicatedExampleException : BusinessException
{
    public DuplicatedExampleException(IStringLocalizer localizer) : base(localizer["message:validation:duplicatedExample"])
    {
    }
}

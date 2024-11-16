using CqrsProject.Common.Exceptions;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Common.Exceptions;

public class EmptyApiResponseException : BusinessException
{
    public EmptyApiResponseException(IStringLocalizer localizer) : base(localizer["message:validation:emptyApiResponse"])
    {
    }
}

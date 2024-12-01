using Microsoft.Extensions.Localization;

namespace CqrsProject.Common.Exceptions;

public class DuplicatedEntityException : BusinessException
{
    public DuplicatedEntityException(IStringLocalizer localizer, string entity) : base(localizer["message:validation:duplicatedEntity", entity])
    {
    }
}

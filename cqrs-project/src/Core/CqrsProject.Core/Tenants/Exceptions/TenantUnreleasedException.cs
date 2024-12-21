using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Tenants.Exceptions;

public class TenantUnreleasedException : UnauthorizedAccessException
{
    public TenantUnreleasedException(IStringLocalizer localizer) : base(localizer["message:validation:tenantUnreleased"])
    {
    }
}

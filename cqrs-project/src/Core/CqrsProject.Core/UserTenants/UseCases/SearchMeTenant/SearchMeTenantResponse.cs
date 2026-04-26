namespace CqrsProject.Core.UserTenants.UseCases.SearchMeTenant;

public record SearchMeTenantResponse(
    Guid Id,
    string TenantName
);

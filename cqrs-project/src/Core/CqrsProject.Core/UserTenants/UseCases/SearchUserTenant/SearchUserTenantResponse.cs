namespace CqrsProject.Core.UserTenants.UseCases.SearchUserTenant;

public record SearchUserTenantResponse(
    Guid UserId,
    Guid TenantId,
    string UserName,
    string TenantName
);

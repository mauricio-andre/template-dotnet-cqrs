namespace CqrsProject.Core.UserTenants.Responses;

public record UserTenantResponse(
    Guid UserId,
    Guid TenantId,
    string UserName,
    string TenantName
);

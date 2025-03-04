namespace CqrsProject.Core.UserTenants.Responses;

public record MeTenantResponse(
    Guid Id,
    string TenantName
);

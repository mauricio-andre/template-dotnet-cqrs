namespace CqrsProject.Core.Identity.Responses;

public record UserTenantResponse(
    Guid UserId,
    Guid TenantId,
    string UserName,
    string TenantName
);

namespace CqrsProject.Core.Tenants.Responses;

public record TenantResponse(
    Guid Id,
    string Nome,
    bool IsDeleted
);

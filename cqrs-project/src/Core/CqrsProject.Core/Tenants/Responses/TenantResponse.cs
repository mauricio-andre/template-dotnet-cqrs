namespace CqrsProject.Core.Responses;

public record TenantResponse(
    Guid Id,
    string Nome,
    bool IsDeleted
);

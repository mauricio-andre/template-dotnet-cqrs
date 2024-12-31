using CqrsProject.Common.Queries;

namespace CqrsProject.App.RestServer.V1.Tenants.Dtos;

public record SearchUserTenantRequestDto(
    string? UserName,
    int? Take,
    int? Skip,
    string? SortBy
);

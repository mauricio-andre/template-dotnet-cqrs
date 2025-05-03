using CqrsProject.Common.Queries;

namespace CqrsProject.App.RestServer.Endpoints.V1.Users.Dtos;

public record SearchUserTenantRequestDto(
    string? TenantName,
    int? Take,
    int? Skip,
    string? SortBy
);

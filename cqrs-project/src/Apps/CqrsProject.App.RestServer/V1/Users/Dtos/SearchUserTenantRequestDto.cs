using CqrsProject.Common.Queries;

namespace CqrsProject.App.RestServer.V1.Users.Dtos;

public record SearchUserTenantRequestDto(
    string? TenantName,
    int? Take,
    int? Skip,
    string? SortBy
);

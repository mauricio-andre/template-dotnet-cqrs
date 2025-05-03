using CqrsProject.Common.Queries;

namespace CqrsProject.App.RestServer.Endpoints.V1.Users.Dtos;

public record SearchUserRoleRequestDto(
    string? RoleName,
    int? Take,
    int? Skip,
    string? SortBy
);

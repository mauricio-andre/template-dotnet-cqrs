using CqrsProject.Common.Queries;

namespace CqrsProject.App.RestServer.V1.Roles.Dtos;

public record SearchRoleClaimPermissionRequestDto(
    string? Name,
    int? Take,
    int? Skip,
    string? SortBy
): IPageableQuery, ISortableQuery;

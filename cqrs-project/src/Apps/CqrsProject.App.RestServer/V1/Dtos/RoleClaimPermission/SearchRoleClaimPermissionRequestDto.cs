using CqrsProject.Common.Queries;

namespace CqrsProject.App.RestServer.V1.Dtos;

public record SearchRoleClaimPermissionRequestDto(
    string? Name,
    int? Take,
    int? Skip,
    string? SortBy
): IPageableQuery, ISortableQuery;

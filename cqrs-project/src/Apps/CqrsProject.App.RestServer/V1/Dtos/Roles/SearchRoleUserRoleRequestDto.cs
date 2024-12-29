using CqrsProject.Common.Queries;

namespace CqrsProject.App.RestServer.V1.Dtos;

public record SearchRoleUserRoleRequestDto(
    Guid? UserId,
    string? UserName,
    int? Take,
    int? Skip,
    string? SortBy
) : IPageableQuery, ISortableQuery;

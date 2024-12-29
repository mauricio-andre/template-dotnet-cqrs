using CqrsProject.Common.Queries;

namespace CqrsProject.App.RestServer.V1.Dtos;

public record SearchTenantUserTenantRequestDto(
    string? UserName,
    Guid? UserId,
    int? Take,
    int? Skip,
    string? SortBy
) : IPageableQuery, ISortableQuery;

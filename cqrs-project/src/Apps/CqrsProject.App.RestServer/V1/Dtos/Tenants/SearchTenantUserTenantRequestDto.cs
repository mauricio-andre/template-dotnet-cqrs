using CqrsProject.Common.Queries;

namespace CqrsProject.App.RestServer.V1.Dtos;

public record SearchTenantUserTenantRequestDto(
    string? UserName,
    int? Take,
    int? Skip,
    string? SortBy
) : IPageableQuery, ISortableQuery;

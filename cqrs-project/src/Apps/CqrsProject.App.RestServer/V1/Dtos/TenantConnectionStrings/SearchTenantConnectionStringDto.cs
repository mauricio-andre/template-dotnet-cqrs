using CqrsProject.Common.Queries;

namespace CqrsProject.App.RestServer.V1.Dtos;

public record SearchTenantConnectionStringDto(
    string? ConnectionName,
    int? Take,
    int? Skip,
    string? SortBy
) : IPageableQuery, ISortableQuery;

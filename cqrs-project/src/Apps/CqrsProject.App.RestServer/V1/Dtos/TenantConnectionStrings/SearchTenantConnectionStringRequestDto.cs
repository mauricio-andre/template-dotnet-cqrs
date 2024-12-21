using CqrsProject.Common.Queries;

namespace CqrsProject.App.RestServer.V1.Dtos;

public record SearchTenantConnectionStringRequestDto(
    string? ConnectionName,
    int? Take,
    int? Skip,
    string? SortBy
) : IPageableQuery, ISortableQuery;

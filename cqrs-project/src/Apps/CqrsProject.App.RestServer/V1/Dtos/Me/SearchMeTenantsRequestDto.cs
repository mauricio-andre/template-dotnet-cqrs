using CqrsProject.Common.Queries;

namespace CqrsProject.App.RestServer.V1.Dtos;

public record SearchMeTenantsRequestDto(
    string? UserName,
    string? TenantName,
    int? Take,
    int? Skip,
    string? SortBy
) : IPageableQuery, ISortableQuery;

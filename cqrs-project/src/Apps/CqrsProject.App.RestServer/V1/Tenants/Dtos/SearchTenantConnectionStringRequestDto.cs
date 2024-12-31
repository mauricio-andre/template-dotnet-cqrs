using CqrsProject.Common.Queries;

namespace CqrsProject.App.RestServer.V1.Tenants.Dtos;

public record SearchTenantConnectionStringRequestDto(
    string? ConnectionName,
    int? Take,
    int? Skip,
    string? SortBy
);

using CqrsProject.Common.Queries;
using CqrsProject.Common.Responses;
using MediatR;

namespace CqrsProject.Core.UserTenants.UseCases.SearchMeTenant;

public record SearchMeTenantQuery(
    string? TenantName,
    IList<Guid>? TenantIdList,
    int? Take,
    int? Skip,
    string? SortBy
) : IPageableQuery, ISortableQuery, IRequest<CollectionResponse<SearchMeTenantResponse>>;

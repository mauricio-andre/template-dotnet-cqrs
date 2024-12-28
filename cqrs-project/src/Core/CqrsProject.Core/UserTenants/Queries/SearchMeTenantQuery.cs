using CqrsProject.Common.Queries;
using CqrsProject.Common.Responses;
using CqrsProject.Core.UserTenants.Responses;
using MediatR;

namespace CqrsProject.Core.UserTenants.Queries;

public record SearchMeTenantQuery(
    string? TenantName,
    IList<Guid>? TenantIdList,
    int? Take,
    int? Skip,
    string? SortBy
) : IPageableQuery, ISortableQuery, IRequest<CollectionResponse<MeTenantResponse>>;

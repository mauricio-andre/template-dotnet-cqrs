using CqrsProject.Common.Queries;
using CqrsProject.Common.Responses;
using CqrsProject.Core.UserTenants.Responses;
using MediatR;

namespace CqrsProject.Core.UserTenants.Queries;

public record SearchUserTenantQuery(
    string? UserName,
    string? TenantName,
    IList<Guid>? UserIdList,
    IList<Guid>? TenantIdList,
    int? Take,
    int? Skip,
    string? SortBy
) : IPageableQuery, ISortableQuery, IRequest<CollectionResponse<UserTenantResponse>>;

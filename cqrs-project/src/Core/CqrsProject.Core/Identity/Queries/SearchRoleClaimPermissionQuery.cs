using CqrsProject.Common.Queries;
using CqrsProject.Common.Responses;
using MediatR;

namespace CqrsProject.Core.Identity.Queries;

public record SearchRoleClaimPermissionQuery(
    Guid RoleId,
    string? Name,
    int? Take,
    int? Skip,
    string? SortBy
) : IPageableQuery, ISortableQuery, IRequest<CollectionResponse<string>>;

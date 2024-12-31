using CqrsProject.Common.Queries;
using CqrsProject.Common.Responses;
using CqrsProject.Core.Identity.Responses;
using MediatR;

namespace CqrsProject.Core.Identity.Queries;

public record SearchRoleQuery(
    string? Name,
    int? Take,
    int? Skip,
    string? SortBy
): IPageableQuery, ISortableQuery, IRequest<CollectionResponse<RoleResponse>>;

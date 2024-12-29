using CqrsProject.Common.Queries;
using CqrsProject.Common.Responses;
using CqrsProject.Core.UserRoles.Responses;
using MediatR;

namespace CqrsProject.Core.UserRoles.Queries;

public record SearchUserRoleQuery(
    Guid? UserId,
    Guid? RoleId,
    string? UserName,
    string? RoleName,
    int? Take,
    int? Skip,
    string? SortBy
): IPageableQuery, ISortableQuery, IRequest<CollectionResponse<UserRoleResponse>>;

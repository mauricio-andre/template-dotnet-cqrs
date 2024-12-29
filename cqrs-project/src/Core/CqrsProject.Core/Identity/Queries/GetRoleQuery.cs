using CqrsProject.Core.Identity.Responses;
using MediatR;

namespace CqrsProject.Core.Identity.Queries;

public record GetRoleQuery(
    Guid Id
) : IRequest<RoleResponse>;

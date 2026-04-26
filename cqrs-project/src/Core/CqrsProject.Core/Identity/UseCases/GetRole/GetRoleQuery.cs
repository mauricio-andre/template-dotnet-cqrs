using CqrsProject.Core.Identity.Responses;
using MediatR;

namespace CqrsProject.Core.Identity.UseCases.GetRole;

public record GetRoleQuery(
    Guid Id
) : IRequest<RoleResponse>;

using CqrsProject.Core.Identity.Responses;
using MediatR;

namespace CqrsProject.Core.Identity.Commands;

public record UpdateRoleCommand(
    Guid Id,
    string Name
) : IRequest<RoleResponse>;

using CqrsProject.Core.Identity.Responses;
using MediatR;

namespace CqrsProject.Core.Identity.Commands;

public record CreateRoleCommand(
    string Name
) : IRequest<RoleResponse>;

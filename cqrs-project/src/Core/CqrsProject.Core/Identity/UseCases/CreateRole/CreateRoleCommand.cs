using CqrsProject.Core.Identity.Responses;
using MediatR;

namespace CqrsProject.Core.Identity.UseCases.CreateRole;

public record CreateRoleCommand(
    string Name
) : IRequest<RoleResponse>;

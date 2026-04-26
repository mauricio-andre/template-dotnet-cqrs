using MediatR;

namespace CqrsProject.Core.Identity.UseCases.CreateUserRole;

public record CreateUserRoleCommand(
    Guid UserId,
    Guid RoleId
) : IRequest;

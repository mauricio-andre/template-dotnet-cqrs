using MediatR;

namespace CqrsProject.Core.Identity.UseCases.RemoveUserRole;

public record RemoveUserRoleCommand(
    Guid UserId,
    Guid RoleId
) : IRequest;

using MediatR;

namespace CqrsProject.Core.Identity.Commands;

public record RemoveRoleCommand(
    Guid Id
) : IRequest;

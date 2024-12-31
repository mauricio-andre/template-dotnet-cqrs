using MediatR;

namespace CqrsProject.Core.Identity.Commands;

public record RemoveUserCommand(
    Guid Id
) : IRequest;

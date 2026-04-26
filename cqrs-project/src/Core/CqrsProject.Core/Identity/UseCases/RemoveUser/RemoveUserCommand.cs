using MediatR;

namespace CqrsProject.Core.Identity.UseCases.RemoveUser;

public record RemoveUserCommand(
    Guid Id
) : IRequest;

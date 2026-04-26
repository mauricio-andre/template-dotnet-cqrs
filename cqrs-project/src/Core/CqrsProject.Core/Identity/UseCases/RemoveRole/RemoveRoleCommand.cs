using MediatR;

namespace CqrsProject.Core.Identity.UseCases.RemoveRole;

public record RemoveRoleCommand(
    Guid Id
) : IRequest;

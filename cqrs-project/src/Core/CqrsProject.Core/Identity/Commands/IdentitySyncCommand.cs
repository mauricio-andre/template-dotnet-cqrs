using MediatR;

namespace CqrsProject.Core.Identity.Commands;

public record IdentitySyncCommand(
    string NameIdentifier,
    string AccessToken
) : IRequest;

using MediatR;

namespace CqrsProject.Core.Commands;

public record IdentitySyncCommand(
    string NameIdentifier,
    string AccessToken
) : IRequest;

using MediatR;

namespace CqrsProject.Core.Identity.UseCases.IdentitySync;

public record IdentitySyncCommand(
    string NameIdentifier,
    string AccessToken
) : IRequest;

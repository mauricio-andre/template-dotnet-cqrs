using MediatR;

namespace CqrsProject.Core.Commands;

public record RemoveTenantCommand(
    Guid Id
) : IRequest;

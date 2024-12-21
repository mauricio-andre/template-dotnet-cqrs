using MediatR;

namespace CqrsProject.Core.Tenants.Commands;

public record RemoveTenantCommand(
    Guid Id
) : IRequest;

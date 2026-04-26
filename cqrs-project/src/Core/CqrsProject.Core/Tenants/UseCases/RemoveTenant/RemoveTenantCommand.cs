using MediatR;

namespace CqrsProject.Core.Tenants.UseCases.RemoveTenant;

public record RemoveTenantCommand(
    Guid Id
) : IRequest;

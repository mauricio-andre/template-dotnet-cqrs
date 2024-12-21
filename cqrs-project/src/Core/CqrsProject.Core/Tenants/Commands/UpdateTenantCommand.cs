using CqrsProject.Core.Tenants.Responses;
using MediatR;

namespace CqrsProject.Core.Tenants.Commands;

public record UpdateTenantCommand(
    Guid Id,
    string Name
) : IRequest<TenantResponse>;

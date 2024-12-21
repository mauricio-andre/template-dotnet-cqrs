using CqrsProject.Core.Tenants.Responses;
using MediatR;

namespace CqrsProject.Core.Tenants.Commands;

public record CreateTenantCommand(
    string Name
) : IRequest<TenantResponse>;

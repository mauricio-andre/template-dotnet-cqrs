using CqrsProject.Core.Tenants.Responses;
using MediatR;

namespace CqrsProject.Core.Tenants.Commands;

public record CreateTenantConnectionStringCommand(
    string ConnectionName,
    string KeyName,
    Guid TenantId
) : IRequest<TenantConnectionStringResponse>;

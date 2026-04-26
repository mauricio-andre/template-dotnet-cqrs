using CqrsProject.Core.Tenants.Responses;
using MediatR;

namespace CqrsProject.Core.Tenants.UseCases.CreateTenantConnectionString;

public record CreateTenantConnectionStringCommand(
    string ConnectionName,
    string KeyName,
    Guid TenantId
) : IRequest<TenantConnectionStringResponse>;

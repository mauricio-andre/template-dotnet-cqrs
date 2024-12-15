using CqrsProject.Core.Responses;
using MediatR;

namespace CqrsProject.Core.Commands;

public record CreateTenantConnectionStringCommand(
    string ConnectionName,
    string KeyName,
    Guid TenantId
) : IRequest<TenantConnectionStringResponse>;

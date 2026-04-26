using CqrsProject.Core.Tenants.Responses;
using MediatR;

namespace CqrsProject.Core.Tenants.UseCases.CreateTenant;

public record CreateTenantCommand(
    string Name
) : IRequest<TenantResponse>;

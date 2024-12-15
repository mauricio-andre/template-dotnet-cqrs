using CqrsProject.Core.Responses;
using MediatR;

namespace CqrsProject.Core.Commands;

public record UpdateTenantCommand(
    Guid Id,
    string Name
) : IRequest<TenantResponse>;

using CqrsProject.Core.Responses;
using MediatR;

namespace CqrsProject.Core.Commands;

public record CreateTenantCommand(
    string Name
) : IRequest<TenantResponse>;

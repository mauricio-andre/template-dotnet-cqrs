using CqrsProject.Core.UserTenants.Responses;
using MediatR;

namespace CqrsProject.Core.UserTenants.Commands;

public record CreateUserTenantCommand(
    Guid UserId,
    Guid TenantId
) : IRequest<UserTenantResponse>;

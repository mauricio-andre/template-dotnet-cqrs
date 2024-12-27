using CqrsProject.Core.Identity.Responses;
using MediatR;

namespace CqrsProject.Core.Identity.Commands;

public record CreateUserTenantCommand(
    Guid UserId,
    Guid TenantId
) : IRequest<UserTenantResponse>;

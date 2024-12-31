using CqrsProject.Core.Identity.Responses;
using MediatR;

namespace CqrsProject.Core.Identity.Commands;

public record UpdateUserCommand(
    Guid Id,
    string UserName,
    string Email,
    string? PhoneNumber
) : IRequest<UserResponse>;

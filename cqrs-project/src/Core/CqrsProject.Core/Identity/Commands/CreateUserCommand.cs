using CqrsProject.Core.Identity.Responses;
using MediatR;

namespace CqrsProject.Core.Identity.Commands;

public record CreateUserCommand(
    string UserName,
    string Email,
    string? PhoneNumber
) : IRequest<UserResponse>;

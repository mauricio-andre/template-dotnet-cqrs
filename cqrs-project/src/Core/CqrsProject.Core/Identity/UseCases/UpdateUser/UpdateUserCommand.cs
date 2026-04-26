using CqrsProject.Core.Identity.Responses;
using MediatR;

namespace CqrsProject.Core.Identity.UseCases.UpdateUser;

public record UpdateUserCommand(
    Guid Id,
    string UserName,
    string Email,
    string? PhoneNumber
) : IRequest<UserResponse>;

using MediatR;

namespace CqrsProject.Core.Identity.UseCases.CreateUser;

public record CreateUserEvent(
    string UserName,
    string Email
) : INotification;

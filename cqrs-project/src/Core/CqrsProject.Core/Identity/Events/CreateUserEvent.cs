using MediatR;

namespace CqrsProject.Core.Identity.Events;

public record CreateUserEvent(
    string UserName,
    string Email
) : INotification;

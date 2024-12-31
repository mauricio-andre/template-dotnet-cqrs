using MediatR;

namespace CqrsProject.Core.Identity.Events;

public record UpdateUserEvent(
    Guid Id,
    string UserName,
    string Email
) : INotification;

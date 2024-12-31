using MediatR;

namespace CqrsProject.Core.Identity.Events;

public record UpdateRoleEvent(
    Guid Id,
    string Name
) : INotification;

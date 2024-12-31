using MediatR;

namespace CqrsProject.Core.Identity.Events;

public record CreateRoleEvent(
    string Name
) : INotification;

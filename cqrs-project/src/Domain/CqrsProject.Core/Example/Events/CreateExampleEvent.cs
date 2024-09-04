using MediatR;

namespace CqrsProject.Core.Events;

public record CreateExampleEvent(
    string name
) : INotification;

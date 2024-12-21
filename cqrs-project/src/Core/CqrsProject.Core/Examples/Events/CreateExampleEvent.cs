using MediatR;

namespace CqrsProject.Core.Examples.Events;

public record CreateExampleEvent(
    string Name
) : INotification;

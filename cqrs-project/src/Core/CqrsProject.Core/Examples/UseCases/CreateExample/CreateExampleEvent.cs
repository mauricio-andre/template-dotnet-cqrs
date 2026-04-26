using MediatR;

namespace CqrsProject.Core.Examples.UseCases.CreateExample;

public record CreateExampleEvent(
    string Name
) : INotification;

using MediatR;

namespace CqrsProject.Core.Examples.Commands;

public record RemoveExampleCommand(
    int Id
) : IRequest;

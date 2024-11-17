using MediatR;

namespace CqrsProject.Core.Commands;

public record RemoveExampleCommand(
    int Id
) : IRequest;

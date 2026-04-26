using MediatR;

namespace CqrsProject.Core.Examples.UseCases.RemoveExample;

public record RemoveExampleCommand(
    int Id
) : IRequest;

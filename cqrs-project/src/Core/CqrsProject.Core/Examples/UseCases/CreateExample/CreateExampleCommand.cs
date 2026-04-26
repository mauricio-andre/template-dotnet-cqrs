using CqrsProject.Core.Examples.Responses;
using MediatR;

namespace CqrsProject.Core.Examples.UseCases.CreateExample;

public record CreateExampleCommand(
    string Name
) : IRequest<ExampleResponse>;

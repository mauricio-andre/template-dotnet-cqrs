using CqrsProject.Core.Examples.Responses;
using MediatR;

namespace CqrsProject.Core.Examples.Commands;

public record CreateExampleCommand(
    string Name
) : IRequest<ExampleResponse>;

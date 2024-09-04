using CqrsProject.Core.Responses;
using MediatR;

namespace CqrsProject.Core.Commands;

public record CreateExampleCommand(
    string Name
) : IRequest<ExampleResponse>;

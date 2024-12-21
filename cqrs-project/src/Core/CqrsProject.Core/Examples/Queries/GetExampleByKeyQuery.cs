using CqrsProject.Core.Examples.Responses;
using MediatR;

namespace CqrsProject.Core.Examples.Queries;

public record GetExampleByKeyQuery(
    int Id
) : IRequest<ExampleResponse>;

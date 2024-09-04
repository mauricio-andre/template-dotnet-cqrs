using CqrsProject.Core.Responses;
using MediatR;

namespace CqrsProject.Core.Queries;

public record GetExampleByKeyQuery(
    int Id
) : IRequest<ExampleResponse>;

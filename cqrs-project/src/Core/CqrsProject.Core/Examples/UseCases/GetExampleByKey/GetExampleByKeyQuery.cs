using CqrsProject.Core.Examples.Responses;
using MediatR;

namespace CqrsProject.Core.Examples.UseCases.GetExampleByKey;

public record GetExampleByKeyQuery(
    int Id
) : IRequest<ExampleResponse>;

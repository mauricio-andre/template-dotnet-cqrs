using CqrsProject.Common.Queries;
using CqrsProject.Common.Responses;
using CqrsProject.Core.Examples.Responses;
using MediatR;

namespace CqrsProject.Core.Examples.UseCases.SearchExample;

public record SearchExampleQuery(
    string? Term,
    int? Take,
    int? Skip,
    string? SortBy
) : IPageableQuery, ISortableQuery, IRequest<CollectionResponse<ExampleResponse>>;

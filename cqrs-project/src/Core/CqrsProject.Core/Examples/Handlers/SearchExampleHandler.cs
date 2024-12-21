using CqrsProject.Common.Extensions;
using CqrsProject.Common.Responses;
using CqrsProject.Core.Data;
using CqrsProject.Core.Examples.Entities;
using CqrsProject.Core.Examples.Queries;
using CqrsProject.Core.Examples.Responses;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Examples.Handlers;

public class SearchExampleHandler : IRequestHandler<SearchExampleQuery, CollectionResponse<ExampleResponse>>
{
    private readonly CoreDbContext _coreDbContext;
    private readonly IValidator<SearchExampleQuery> _validator;

    public SearchExampleHandler(
        CoreDbContext coreDbContext,
        IValidator<SearchExampleQuery> validator)
    {
        _coreDbContext = coreDbContext;
        _validator = validator;
    }

    public async Task<CollectionResponse<ExampleResponse>> Handle(
        SearchExampleQuery request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var query = CreateSearchQuery(request).AsNoTracking();
        var totalCount = await query.CountAsync();

        query = query
            .ApplySorting(request)
            .ApplyPagination(request);

        var items = MapToResponse(query).AsAsyncEnumerable();
        return new CollectionResponse<ExampleResponse>(items, totalCount);
    }

    private IQueryable<Example> CreateSearchQuery(SearchExampleQuery request)
    {
        return _coreDbContext.Examples
            .WhereIf(
                !string.IsNullOrEmpty(request.Term),
                example => example.Name.ToLower().Contains(request.Term!.ToLower()));
    }

    private static IQueryable<ExampleResponse> MapToResponse(IQueryable<Example> query)
        => query.Select(entity => new ExampleResponse(entity.Id, entity.Name));
}

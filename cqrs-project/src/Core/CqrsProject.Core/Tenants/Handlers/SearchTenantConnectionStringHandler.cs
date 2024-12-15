using CqrsProject.Common.Extensions;
using CqrsProject.Common.Responses;
using CqrsProject.Core.Data;
using CqrsProject.Core.Queries;
using CqrsProject.Core.Responses;
using CqrsProject.Core.Tenants;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Handlers;

public class SearchTenantConnectionStringHandler : IRequestHandler<
    SearchTenantConnectionStringQuery,
    CollectionResponse<TenantConnectionStringResponse>>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IValidator<SearchTenantConnectionStringQuery> _validator;

    public SearchTenantConnectionStringHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<SearchTenantConnectionStringQuery> validator)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _validator = validator;
    }

    public async Task<CollectionResponse<TenantConnectionStringResponse>> Handle(
        SearchTenantConnectionStringQuery request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var query = CreateSearchQuery(request).AsNoTracking();
        var totalCount = await query.CountAsync();

        query = query
            .ApplySorting(request)
            .ApplyPagination(request);

        var items = MapToResponse(query).AsAsyncEnumerable();
        return new CollectionResponse<TenantConnectionStringResponse>(items, totalCount);
    }

    private IQueryable<TenantConnectionString> CreateSearchQuery(SearchTenantConnectionStringQuery request)
    {
        return _administrationDbContext.TenantConnectionStrings
            .Where(entity => entity.TenantId == request.TenantId)
            .WhereIf(
                !string.IsNullOrEmpty(request.ConnectionName),
                entity => entity.ConnectionName.ToLower().Contains(request.ConnectionName!.ToLower()));
    }

    private static IQueryable<TenantConnectionStringResponse> MapToResponse(IQueryable<TenantConnectionString> query)
        => query.Select(entity => new TenantConnectionStringResponse(
            entity.Id,
            entity.TenantId,
            entity.ConnectionName,
            entity.KeyName));
}

using CqrsProject.Common.Extensions;
using CqrsProject.Common.Responses;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.Tenants.Responses;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Tenants.UseCases.SearchTenantConnectionString;

public class SearchTenantConnectionStringHandler : IRequestHandler<
    SearchTenantConnectionStringQuery,
    CollectionResponse<TenantConnectionStringResponse>>
{
    private readonly IDbContextFactory<AdministrationDbContext> _dbContextFactory;
    private readonly IValidator<SearchTenantConnectionStringQuery> _validator;

    public SearchTenantConnectionStringHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<SearchTenantConnectionStringQuery> validator)
    {
        _dbContextFactory = dbContextFactory;
        _validator = validator;
    }

    public async Task<CollectionResponse<TenantConnectionStringResponse>> Handle(
        SearchTenantConnectionStringQuery request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var administrationDbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var query = CreateSearchQuery(administrationDbContext, request).AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);

        query = query
            .ApplySorting(request)
            .ApplyPagination(request);

        var items = MapToResponse(query).AsAsyncEnumerable();
        return new CollectionResponse<TenantConnectionStringResponse>(items, totalCount);
    }

    private static IQueryable<TenantConnectionString> CreateSearchQuery(
        AdministrationDbContext administrationDbContext,
        SearchTenantConnectionStringQuery request)
    {
        return administrationDbContext.TenantConnectionStrings
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

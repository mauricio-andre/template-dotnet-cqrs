using CqrsProject.Common.Extensions;
using CqrsProject.Common.Responses;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.Tenants.Queries;
using CqrsProject.Core.Tenants.Responses;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Tenants.Handlers;

public class SearchTenantHandler : IRequestHandler<SearchTenantQuery, CollectionResponse<TenantResponse>>
{
    private readonly IDbContextFactory<AdministrationDbContext> _dbContextFactory;
    private readonly IValidator<SearchTenantQuery> _validator;

    public SearchTenantHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<SearchTenantQuery> validator)
    {
        _dbContextFactory = dbContextFactory;
        _validator = validator;
    }

    public async Task<CollectionResponse<TenantResponse>> Handle(
        SearchTenantQuery request,
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
        return new CollectionResponse<TenantResponse>(items, totalCount);
    }

    private static IQueryable<Tenant> CreateSearchQuery(
        AdministrationDbContext administrationDbContext,
        SearchTenantQuery request)
    {
        return administrationDbContext.Tenants
            .WhereIf(
                !string.IsNullOrEmpty(request.Name),
                tenant => tenant.Name.ToLower().Contains(request.Name!.ToLower()))
            .WhereIf(
                request.IsDeleted.HasValue,
                tenant => tenant.IsDeleted == request.IsDeleted);
    }

    private static IQueryable<TenantResponse> MapToResponse(IQueryable<Tenant> query)
        => query.Select(entity => new TenantResponse(entity.Id, entity.Name, entity.IsDeleted));
}

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
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IValidator<SearchTenantQuery> _validator;

    public SearchTenantHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<SearchTenantQuery> validator)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _validator = validator;
    }

    public async Task<CollectionResponse<TenantResponse>> Handle(
        SearchTenantQuery request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var query = CreateSearchQuery(request).AsNoTracking();
        var totalCount = await query.CountAsync();

        query = query
            .ApplySorting(request)
            .ApplyPagination(request);

        var items = MapToResponse(query).AsAsyncEnumerable();
        return new CollectionResponse<TenantResponse>(items, totalCount);
    }

    private IQueryable<Tenant> CreateSearchQuery(SearchTenantQuery request)
    {
        return _administrationDbContext.Tenants
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

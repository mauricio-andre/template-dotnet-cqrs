using CqrsProject.Common.Extensions;
using CqrsProject.Common.Responses;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Interfaces;
using CqrsProject.Core.UserTenants.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.UserTenants.UseCases.SearchMeTenant;

public class SearchMeTenantHandler : IRequestHandler<SearchMeTenantQuery, CollectionResponse<SearchMeTenantResponse>>
{
    private readonly IDbContextFactory<AdministrationDbContext> _dbContextFactory;
    private readonly IValidator<SearchMeTenantQuery> _validator;
    private readonly ICurrentIdentity _currentIdentity;

    public SearchMeTenantHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<SearchMeTenantQuery> validator,
        ICurrentIdentity currentIdentity)
    {
        _dbContextFactory = dbContextFactory;
        _validator = validator;
        _currentIdentity = currentIdentity;
    }

    public async Task<CollectionResponse<SearchMeTenantResponse>> Handle(
        SearchMeTenantQuery request,
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
        return new CollectionResponse<SearchMeTenantResponse>(items, totalCount);
    }

    private IQueryable<UserTenant> CreateSearchQuery(
        AdministrationDbContext administrationDbContext,
        SearchMeTenantQuery request)
    {
        var userId = _currentIdentity.GetLocalIdentityId();
        return administrationDbContext.UserTenants
            .Where(tenant => !tenant.Tenant!.IsDeleted)
            .Where(tenant => !tenant.User!.IsDeleted)
            .Where(tenant => tenant.UserId == userId)
            .WhereIf(
                request.TenantIdList?.Any() ?? false,
                tenant => request.TenantIdList!.Contains(tenant.TenantId))
            .WhereIf(
                !string.IsNullOrEmpty(request.TenantName),
                tenant => tenant.Tenant!.Name!.ToLower().Contains(request.TenantName!.ToLower()));
    }

    private static IQueryable<SearchMeTenantResponse> MapToResponse(IQueryable<UserTenant> query)
        => query.Select(entity => new SearchMeTenantResponse(
            entity.TenantId,
            entity.Tenant!.Name));
}

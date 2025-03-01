using CqrsProject.Common.Extensions;
using CqrsProject.Common.Responses;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Interfaces;
using CqrsProject.Core.UserTenants.Entities;
using CqrsProject.Core.UserTenants.Queries;
using CqrsProject.Core.UserTenants.Responses;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.UserTenants.Handlers;

public class SearchMeTenantHandler : IRequestHandler<SearchMeTenantQuery, CollectionResponse<MeTenantResponse>>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IValidator<SearchMeTenantQuery> _validator;
    private readonly ICurrentIdentity _currentIdentity;

    public SearchMeTenantHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<SearchMeTenantQuery> validator,
        ICurrentIdentity currentIdentity)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _validator = validator;
        _currentIdentity = currentIdentity;
    }

    public async Task<CollectionResponse<MeTenantResponse>> Handle(
        SearchMeTenantQuery request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var query = CreateSearchQuery(request).AsNoTracking();
        var totalCount = await query.CountAsync();

        query = query
            .ApplySorting(request)
            .ApplyPagination(request);

        var items = MapToResponse(query).AsAsyncEnumerable();
        return new CollectionResponse<MeTenantResponse>(items, totalCount);
    }

    private IQueryable<UserTenant> CreateSearchQuery(SearchMeTenantQuery request)
    {
        return _administrationDbContext.UserTenants
            .Where(tenant => !tenant.Tenant!.IsDeleted)
            .Where(tenant => !tenant.User!.IsDeleted)
            .Where(tenant => tenant.UserId == _currentIdentity.GetLocalIdentityId())
            .WhereIf(
                request.TenantIdList?.Any() ?? false,
                tenant => request.TenantIdList!.Contains(tenant.TenantId))
            .WhereIf(
                !string.IsNullOrEmpty(request.TenantName),
                tenant => tenant.Tenant!.Name!.ToLower().Contains(request.TenantName!.ToLower()));
    }

    private static IQueryable<MeTenantResponse> MapToResponse(IQueryable<UserTenant> query)
        => query.Select(entity => new MeTenantResponse(
            entity.TenantId,
            entity.Tenant!.Name));
}

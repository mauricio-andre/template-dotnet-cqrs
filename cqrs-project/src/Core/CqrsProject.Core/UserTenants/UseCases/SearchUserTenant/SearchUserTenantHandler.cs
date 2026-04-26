using CqrsProject.Common.Extensions;
using CqrsProject.Common.Responses;
using CqrsProject.Core.Data;
using CqrsProject.Core.UserTenants.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.UserTenants.UseCases.SearchUserTenant;

public class SearchUserTenantHandler : IRequestHandler<SearchUserTenantQuery, CollectionResponse<SearchUserTenantResponse>>
{
    private readonly IDbContextFactory<AdministrationDbContext> _dbContextFactory;
    private readonly IValidator<SearchUserTenantQuery> _validator;

    public SearchUserTenantHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<SearchUserTenantQuery> validator)
    {
        _dbContextFactory = dbContextFactory;
        _validator = validator;
    }

    public async Task<CollectionResponse<SearchUserTenantResponse>> Handle(
        SearchUserTenantQuery request,
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
        return new CollectionResponse<SearchUserTenantResponse>(items, totalCount);
    }

    private static IQueryable<UserTenant> CreateSearchQuery(
        AdministrationDbContext administrationDbContext,
        SearchUserTenantQuery request)
    {
        return administrationDbContext.UserTenants
            .Where(tenant => !tenant.Tenant!.IsDeleted)
            .Where(tenant => !tenant.User!.IsDeleted)
            .WhereIf(
                request.UserId.HasValue,
                tenant => request.UserId == tenant.UserId)
            .WhereIf(
                request.TenantId.HasValue,
                tenant => request.TenantId == tenant.TenantId)
            .WhereIf(
                !string.IsNullOrEmpty(request.UserName),
                tenant => tenant.User!.UserName!.ToLower().Contains(request.UserName!.ToLower()))
            .WhereIf(
                !string.IsNullOrEmpty(request.TenantName),
                tenant => tenant.Tenant!.Name!.ToLower().Contains(request.TenantName!.ToLower()));
    }

    private static IQueryable<SearchUserTenantResponse> MapToResponse(IQueryable<UserTenant> query)
        => query.Select(entity => new SearchUserTenantResponse(
            entity.UserId,
            entity.TenantId,
            entity.User!.UserName!,
            entity.Tenant!.Name));
}

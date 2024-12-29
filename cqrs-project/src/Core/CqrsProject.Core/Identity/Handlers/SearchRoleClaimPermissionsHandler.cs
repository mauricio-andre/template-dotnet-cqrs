using CqrsProject.Common.Consts;
using CqrsProject.Common.Extensions;
using CqrsProject.Common.Responses;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Identity.Handlers;

public class SearchRoleClaimPermissionsHandler : IRequestHandler<SearchRoleClaimPermissionQuery, CollectionResponse<string>>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IValidator<SearchRoleClaimPermissionQuery> _validator;

    public SearchRoleClaimPermissionsHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<SearchRoleClaimPermissionQuery> validator)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _validator = validator;
    }

    public async Task<CollectionResponse<string>> Handle(
        SearchRoleClaimPermissionQuery request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var query = CreateSearchQuery(request).AsNoTracking();
        var totalCount = await query.CountAsync();

        query = query
            .ApplySorting(request)
            .ApplyPagination(request);

        var items = MapToResponse(query).AsAsyncEnumerable();
        return new CollectionResponse<string>(items, totalCount);
    }

    private IQueryable<IdentityRoleClaim<Guid>> CreateSearchQuery(SearchRoleClaimPermissionQuery request)
    {
        return _administrationDbContext.RoleClaims
            .Where(roleClaim => roleClaim.RoleId == request.RoleId)
            .Where(roleClaim => roleClaim.ClaimType == AuthorizationPermissionClaims.ClaimType)
            .WhereIf(
                !string.IsNullOrEmpty(request.Name),
                roleClaim => roleClaim.ClaimValue!.ToLower().Contains(request.Name!.ToLower()));
    }

    private static IQueryable<string> MapToResponse(IQueryable<IdentityRoleClaim<Guid>> query)
        => query.Select(entity => entity.ClaimValue!);
}

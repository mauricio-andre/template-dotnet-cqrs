using CqrsProject.Common.Extensions;
using CqrsProject.Common.Responses;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Identity.Handlers;

public class SearchRoleClaimHandler : IRequestHandler<
    SearchRoleClaimQuery,
    CollectionResponse<KeyValuePair<string, string>>>
{
    private readonly IDbContextFactory<AdministrationDbContext> _dbContextFactory;
    private readonly IValidator<SearchRoleClaimQuery> _validator;

    public SearchRoleClaimHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<SearchRoleClaimQuery> validator)
    {
        _dbContextFactory = dbContextFactory;
        _validator = validator;
    }

    public async Task<CollectionResponse<KeyValuePair<string, string>>> Handle(
        SearchRoleClaimQuery request,
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
        return new CollectionResponse<KeyValuePair<string, string>>(items, totalCount);
    }

    private static IQueryable<IdentityRoleClaim<Guid>> CreateSearchQuery(
        AdministrationDbContext administrationDbContext,
        SearchRoleClaimQuery request)
    {
        return administrationDbContext.RoleClaims
            .Where(roleClaim => roleClaim.RoleId == request.RoleId)
            .WhereIf(
                !string.IsNullOrEmpty(request.ClaimType),
                roleClaim => roleClaim.ClaimType!.ToLower().Contains(request.ClaimType!.ToLower()));
    }

    private static IQueryable<KeyValuePair<string, string>> MapToResponse(
        IQueryable<IdentityRoleClaim<Guid>> query)
        => query
            .Select(claim => new KeyValuePair<string, string>(
                claim.ClaimType!,
                claim.ClaimValue!));
}

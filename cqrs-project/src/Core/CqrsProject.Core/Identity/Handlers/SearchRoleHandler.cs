using CqrsProject.Common.Extensions;
using CqrsProject.Common.Responses;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Queries;
using CqrsProject.Core.Identity.Responses;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Identity.Handlers;

public class SearchRoleHandler : IRequestHandler<SearchRoleQuery, CollectionResponse<RoleResponse>>
{
    private readonly IValidator<SearchRoleQuery> _validator;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public SearchRoleHandler(
        IValidator<SearchRoleQuery> validator,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _validator = validator;
        _roleManager = roleManager;
    }

    public async Task<CollectionResponse<RoleResponse>> Handle(
        SearchRoleQuery request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var query = CreateSearchQuery(request).AsNoTracking();
        var totalCount = await query.CountAsync();

        query = query
            .ApplySorting(request)
            .ApplyPagination(request);

        var items = MapToResponse(query).AsAsyncEnumerable();
        return new CollectionResponse<RoleResponse>(items, totalCount);
    }

    private IQueryable<IdentityRole<Guid>> CreateSearchQuery(SearchRoleQuery request)
    {
        return _roleManager.Roles
            .WhereIf(
                !string.IsNullOrEmpty(request.Name),
                role => role.NormalizedName!.Contains(_roleManager.NormalizeKey(request.Name)!));
    }

    private static IQueryable<RoleResponse> MapToResponse(IQueryable<IdentityRole<Guid>> query)
        => query.Select(entity => new RoleResponse(
            entity.Id,
            entity.Name!));
}

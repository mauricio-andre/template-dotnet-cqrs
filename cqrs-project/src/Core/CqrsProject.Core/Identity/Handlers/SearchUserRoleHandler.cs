using CqrsProject.Common.Extensions;
using CqrsProject.Common.Responses;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Queries;
using CqrsProject.Core.Identity.Responses;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Identity.Handlers;

public class SearchUserRoleHandler : IRequestHandler<SearchUserRoleQuery, CollectionResponse<UserRoleResponse>>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IValidator<SearchUserRoleQuery> _validator;

    public SearchUserRoleHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<SearchUserRoleQuery> validator)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _validator = validator;
    }

    public async Task<CollectionResponse<UserRoleResponse>> Handle(
        SearchUserRoleQuery request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var query = CreateSearchQuery(request).AsNoTracking();
        var totalCount = await query.CountAsync();

        query = query
            .ApplySorting(request)
            .ApplyPagination(request);

        var items = query.AsAsyncEnumerable();
        return new CollectionResponse<UserRoleResponse>(items, totalCount);
    }

    private IQueryable<UserRoleResponse> CreateSearchQuery(SearchUserRoleQuery request)
    {
        return
            from user in _administrationDbContext.Users
                .WhereIf(
                    !string.IsNullOrEmpty(request.UserName),
                    user => user.UserName!.ToLower().Contains(request.UserName!.ToLower()))
                .WhereIf(
                    request.UserId.HasValue,
                    user => user.Id == request.UserId)
            join userRole in _administrationDbContext.UserRoles
                on user.Id equals userRole.UserId
            join role in _administrationDbContext.Roles
                .WhereIf(
                    !string.IsNullOrEmpty(request.RoleName),
                    role => role.Name!.ToLower().Contains(request.RoleName!.ToLower()))
                .WhereIf(
                    request.RoleId.HasValue,
                    role => role.Id == request.RoleId)
                on userRole.RoleId equals role.Id
            where !user.IsDeleted
            select new UserRoleResponse(
                user.Id,
                role.Id,
                user.UserName!,
                role.Name!);
    }
}

using CqrsProject.Common.Extensions;
using CqrsProject.Common.Responses;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Identity.Queries;
using CqrsProject.Core.Identity.Responses;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Identity.Handlers;

public class SearchUserHandler : IRequestHandler<SearchUserQuery, CollectionResponse<UserResponse>>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IValidator<SearchUserQuery> _validator;
    private readonly UserManager<User> _userManager;

    public SearchUserHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<SearchUserQuery> validator,
        UserManager<User> userManager)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _validator = validator;
        _userManager = userManager;
    }

    public async Task<CollectionResponse<UserResponse>> Handle(
        SearchUserQuery request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var query = CreateSearchQuery(request).AsNoTracking();
        var totalCount = await query.CountAsync();

        query = query
            .ApplySorting(request)
            .ApplyPagination(request);

        var items = MapToResponse(query).AsAsyncEnumerable();
        return new CollectionResponse<UserResponse>(items, totalCount);
    }

    private IQueryable<User> CreateSearchQuery(SearchUserQuery request)
    {
        return _administrationDbContext.Users
            .WhereIf(
                request.IsDeleted.HasValue,
                entity => entity.IsDeleted == request.IsDeleted)
            .WhereIf(
                !string.IsNullOrEmpty(request.Term),
                entity => entity.NormalizedUserName!.Contains(_userManager.NormalizeName(request.Term)!)
                    || entity.NormalizedEmail!.Contains(_userManager.NormalizeName(request.Term)!));
    }

    private static IQueryable<UserResponse> MapToResponse(IQueryable<User> query)
        => query.Select(entity => new UserResponse(
            entity.Id,
            entity.UserName!,
            entity.Email!,
            entity.PhoneNumber,
            entity.AccessFailedCount,
            entity.CreationTime,
            entity.LastModificationTime,
            entity.IsDeleted));
}

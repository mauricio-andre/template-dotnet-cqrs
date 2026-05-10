using CqrsProject.Core.Data;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.Test.UserTenants;
using CqrsProject.Core.UserTenants.UseCases.SearchMeTenant;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace CqrsProject.Core.Test.UserTenants.UseCases;

public class SearchMeTenantTest
{
    [Fact(DisplayName = "Should search only the current user's tenants")]
    public async Task GivenCurrentUser_WhenHandled_ThenReturnOnlyOwnedTenants()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();

        Guid currentUserId = Guid.NewGuid();
        Guid otherUserId = Guid.NewGuid();
        Guid alphaTenantId = Guid.NewGuid();
        Guid betaTenantId = Guid.NewGuid();
        Guid foreignTenantId = Guid.NewGuid();

        await UserTenantsTestData.AddUserAsync(dbContext, currentUserId, "alice");
        await UserTenantsTestData.AddUserAsync(dbContext, otherUserId, "bob");
        await UserTenantsTestData.AddTenantAsync(dbContext, alphaTenantId, "Alpha Tenant");
        await UserTenantsTestData.AddTenantAsync(dbContext, betaTenantId, "Beta Tenant");
        await UserTenantsTestData.AddTenantAsync(dbContext, foreignTenantId, "Foreign Tenant");
        await UserTenantsTestData.AddUserTenantAsync(dbContext, currentUserId, alphaTenantId);
        await UserTenantsTestData.AddUserTenantAsync(dbContext, currentUserId, betaTenantId);
        await UserTenantsTestData.AddUserTenantAsync(dbContext, otherUserId, foreignTenantId);

        context.CurrentIdentity.GetLocalIdentityId().Returns(currentUserId);

        SearchMeTenantHandler handler = new SearchMeTenantHandler(
            context.AdministrationDbContextFactory,
            new SearchMeTenantValidator(),
            context.CurrentIdentity);

        SearchMeTenantQuery query = new SearchMeTenantQuery(
            "Beta",
            null,
            10,
            0,
            "Tenant.Name");

        var response = await handler.Handle(query, CancellationToken.None);
        List<SearchMeTenantResponse> items = await ToListAsync(response.Items);

        Assert.Equal(1, response.TotalCount);
        Assert.Single(items);
        Assert.Equal(betaTenantId, items[0].Id);
        Assert.Equal("Beta Tenant", items[0].TenantName);
    }

    [Fact(DisplayName = "Should reject tenants from deleted records even for the current user")]
    public async Task GivenDeletedRecords_WhenHandled_ThenExcludeThemFromSearch()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();

        Guid currentUserId = Guid.NewGuid();
        Guid activeTenantId = Guid.NewGuid();
        Guid deletedTenantId = Guid.NewGuid();

        await UserTenantsTestData.AddUserAsync(dbContext, currentUserId, "alice");
        await UserTenantsTestData.AddTenantAsync(dbContext, activeTenantId, "Active Tenant");
        await UserTenantsTestData.AddTenantAsync(dbContext, deletedTenantId, "Deleted Tenant", true);
        await UserTenantsTestData.AddUserTenantAsync(dbContext, currentUserId, activeTenantId);
        await UserTenantsTestData.AddUserTenantAsync(dbContext, currentUserId, deletedTenantId);

        context.CurrentIdentity.GetLocalIdentityId().Returns(currentUserId);

        SearchMeTenantHandler handler = new SearchMeTenantHandler(
            context.AdministrationDbContextFactory,
            new SearchMeTenantValidator(),
            context.CurrentIdentity);

        var response = await handler.Handle(
            new SearchMeTenantQuery(null, null, 100, 0, null),
            CancellationToken.None);

        List<SearchMeTenantResponse> items = await ToListAsync(response.Items);

        Assert.Equal(1, response.TotalCount);
        Assert.Single(items);
        Assert.Equal(activeTenantId, items[0].Id);
    }

    [Theory(DisplayName = "Should reject invalid paging parameters for my tenants")]
    [MemberData(nameof(InvalidPaging))]
    public async Task GivenInvalidPaging_WhenHandled_ThenThrowValidationException(int? take, int? skip)
    {
        using CoreTestContext context = new CoreTestContext();
        context.CurrentIdentity.GetLocalIdentityId().Returns(Guid.NewGuid());

        SearchMeTenantHandler handler = new SearchMeTenantHandler(
            context.AdministrationDbContextFactory,
            new SearchMeTenantValidator(),
            context.CurrentIdentity);

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new SearchMeTenantQuery(null, null, take, skip, null), CancellationToken.None));
    }

    public static IEnumerable<object[]> InvalidPaging()
    {
        yield return new object[] { 0, 0 };
        yield return new object[] { 1001, 0 };
        yield return new object[] { 10, -1 };
    }

    private static async Task<List<SearchMeTenantResponse>> ToListAsync(IAsyncEnumerable<SearchMeTenantResponse> items)
    {
        List<SearchMeTenantResponse> result = new List<SearchMeTenantResponse>();

        await foreach (SearchMeTenantResponse item in items)
        {
            result.Add(item);
        }

        return result;
    }
}


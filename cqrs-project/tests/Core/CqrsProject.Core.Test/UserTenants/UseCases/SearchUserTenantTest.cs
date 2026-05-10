using CqrsProject.Core.Data;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.Test.UserTenants;
using CqrsProject.Core.UserTenants.UseCases.SearchUserTenant;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.UserTenants.UseCases;

public class SearchUserTenantTest
{
    [Fact(DisplayName = "Should search user tenants by tenant name and apply pagination")]
    public async Task GivenTenantNameFilter_WhenHandled_ThenReturnMatchingPage()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();

        Guid aliceId = Guid.NewGuid();
        Guid bobId = Guid.NewGuid();
        Guid alphaTenantId = Guid.NewGuid();
        Guid betaTenantId = Guid.NewGuid();

        await UserTenantsTestData.AddUserAsync(dbContext, aliceId, "alice");
        await UserTenantsTestData.AddUserAsync(dbContext, bobId, "bob");
        await UserTenantsTestData.AddTenantAsync(dbContext, alphaTenantId, "Alpha Tenant");
        await UserTenantsTestData.AddTenantAsync(dbContext, betaTenantId, "Beta Tenant");
        await UserTenantsTestData.AddUserTenantAsync(dbContext, aliceId, betaTenantId);
        await UserTenantsTestData.AddUserTenantAsync(dbContext, bobId, betaTenantId);
        await UserTenantsTestData.AddUserTenantAsync(dbContext, aliceId, alphaTenantId);

        SearchUserTenantHandler handler = new SearchUserTenantHandler(
            context.AdministrationDbContextFactory,
            new SearchUserTenantValidator());

        SearchUserTenantQuery query = new SearchUserTenantQuery(
            null,
            "Beta",
            null,
            null,
            1,
            0,
            "User.UserName");

        var response = await handler.Handle(query, CancellationToken.None);
        List<SearchUserTenantResponse> items = await ToListAsync(response.Items);

        Assert.Equal(2, response.TotalCount);
        Assert.Single(items);
        Assert.Equal(aliceId, items[0].UserId);
        Assert.Equal(betaTenantId, items[0].TenantId);
        Assert.Equal("alice", items[0].UserName);
        Assert.Equal("Beta Tenant", items[0].TenantName);
    }

    [Fact(DisplayName = "Should ignore user tenants linked to deleted users or tenants")]
    public async Task GivenDeletedLinks_WhenHandled_ThenExcludeThemFromSearch()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();

        Guid activeUserId = Guid.NewGuid();
        Guid deletedUserId = Guid.NewGuid();
        Guid activeTenantId = Guid.NewGuid();
        Guid deletedTenantId = Guid.NewGuid();

        await UserTenantsTestData.AddUserAsync(dbContext, activeUserId, "alice");
        await UserTenantsTestData.AddUserAsync(dbContext, deletedUserId, "ghost", true);
        await UserTenantsTestData.AddTenantAsync(dbContext, activeTenantId, "Active Tenant");
        await UserTenantsTestData.AddTenantAsync(dbContext, deletedTenantId, "Deleted Tenant", true);
        await UserTenantsTestData.AddUserTenantAsync(dbContext, activeUserId, activeTenantId);
        await UserTenantsTestData.AddUserTenantAsync(dbContext, deletedUserId, activeTenantId);
        await UserTenantsTestData.AddUserTenantAsync(dbContext, activeUserId, deletedTenantId);

        SearchUserTenantHandler handler = new SearchUserTenantHandler(
            context.AdministrationDbContextFactory,
            new SearchUserTenantValidator());

        var response = await handler.Handle(
            new SearchUserTenantQuery(null, null, null, null, 100, 0, null),
            CancellationToken.None);

        List<SearchUserTenantResponse> items = await ToListAsync(response.Items);

        Assert.Equal(1, response.TotalCount);
        Assert.Single(items);
        Assert.Equal(activeUserId, items[0].UserId);
        Assert.Equal(activeTenantId, items[0].TenantId);
    }

    [Theory(DisplayName = "Should reject invalid paging parameters")]
    [MemberData(nameof(InvalidPaging))]
    public async Task GivenInvalidPaging_WhenHandled_ThenThrowValidationException(int? take, int? skip)
    {
        using CoreTestContext context = new CoreTestContext();
        SearchUserTenantHandler handler = new SearchUserTenantHandler(
            context.AdministrationDbContextFactory,
            new SearchUserTenantValidator());

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new SearchUserTenantQuery(null, null, null, null, take, skip, null), CancellationToken.None));
    }

    public static IEnumerable<object[]> InvalidPaging()
    {
        yield return new object[] { 0, 0 };
        yield return new object[] { 1001, 0 };
        yield return new object[] { 10, -1 };
    }

    private static async Task<List<SearchUserTenantResponse>> ToListAsync(IAsyncEnumerable<SearchUserTenantResponse> items)
    {
        List<SearchUserTenantResponse> result = new List<SearchUserTenantResponse>();

        await foreach (SearchUserTenantResponse item in items)
        {
            result.Add(item);
        }

        return result;
    }
}


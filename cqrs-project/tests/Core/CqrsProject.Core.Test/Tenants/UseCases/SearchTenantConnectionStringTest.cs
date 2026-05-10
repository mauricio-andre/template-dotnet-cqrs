using CqrsProject.Common.Responses;
using CqrsProject.Core.Data;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.Tenants.Responses;
using CqrsProject.Core.Tenants.UseCases.SearchTenantConnectionString;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Tenants.UseCases;

public class SearchTenantConnectionStringTest
{
    [Fact(DisplayName = "Should search tenant connection strings by name and paginate")]
    public async Task GivenTenantConnectionStrings_WhenHandled_ThenReturnMatchingPage()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        Guid tenantId = Guid.NewGuid();
        Guid otherTenantId = Guid.NewGuid();

        await AdministrationTestData.AddTenantAsync(dbContext, tenantId, "Tenant One");
        await AdministrationTestData.AddTenantAsync(dbContext, otherTenantId, "Tenant Two");
        await AdministrationTestData.AddTenantConnectionStringAsync(dbContext, tenantId, "Alpha", "Host=alpha;");
        await AdministrationTestData.AddTenantConnectionStringAsync(dbContext, tenantId, "Beta", "Host=beta;");
        await AdministrationTestData.AddTenantConnectionStringAsync(dbContext, otherTenantId, "Ignored", "Host=ignored;");

        SearchTenantConnectionStringHandler handler = new SearchTenantConnectionStringHandler(
            context.AdministrationDbContextFactory,
            new SearchTenantConnectionStringValidator());

        CollectionResponse<TenantConnectionStringResponse> response = await handler.Handle(
            new SearchTenantConnectionStringQuery(tenantId, "a", 1, 0, "ConnectionName asc"),
            CancellationToken.None);

        List<TenantConnectionStringResponse> items = await ReadItemsAsync(response.Items);

        Assert.Equal(2, response.TotalCount);
        Assert.Single(items);
        Assert.Equal("Alpha", items[0].ConnectionName);
    }

    [Theory(DisplayName = "Should reject invalid tenant connection string searches")]
    [MemberData(nameof(InvalidCommands))]
    public async Task GivenInvalidQuery_WhenHandled_ThenThrowValidationException(
        Guid tenantId,
        int? take,
        int? skip)
    {
        using CoreTestContext context = new CoreTestContext();

        SearchTenantConnectionStringHandler handler = new SearchTenantConnectionStringHandler(
            context.AdministrationDbContextFactory,
            new SearchTenantConnectionStringValidator());

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new SearchTenantConnectionStringQuery(tenantId, null, take, skip, null), CancellationToken.None));
    }

    public static IEnumerable<object[]> InvalidCommands()
    {
        yield return new object[] { Guid.Empty, 10, 0 };
        yield return new object[] { Guid.NewGuid(), 0, 0 };
        yield return new object[] { Guid.NewGuid(), 1001, 0 };
        yield return new object[] { Guid.NewGuid(), 10, -1 };
    }

    private static async Task<List<TenantConnectionStringResponse>> ReadItemsAsync(IAsyncEnumerable<TenantConnectionStringResponse> items)
    {
        List<TenantConnectionStringResponse> result = new List<TenantConnectionStringResponse>();

        await foreach (TenantConnectionStringResponse item in items)
        {
            result.Add(item);
        }

        return result;
    }
}

using CqrsProject.Common.Responses;
using CqrsProject.Core.Data;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.Tenants.Responses;
using CqrsProject.Core.Tenants.UseCases.SearchTenant;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Tenants.UseCases;

public class SearchTenantTest
{
    [Fact(DisplayName = "Should search active tenants by name and apply pagination")]
    public async Task GivenActiveTenants_WhenHandled_ThenReturnMatchingPage()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();

        Tenant alpha = await AdministrationTestData.AddTenantAsync(dbContext, name: "Alpha Tenant");
        await AdministrationTestData.AddTenantAsync(dbContext, name: "Beta Tenant");
        await AdministrationTestData.AddTenantAsync(dbContext, name: "Deleted Tenant", isDeleted: true);

        SearchTenantHandler handler = new SearchTenantHandler(
            context.AdministrationDbContextFactory,
            new SearchTenantValidator());

        CollectionResponse<TenantResponse> response = await handler.Handle(
            new SearchTenantQuery("tenant", 1, 0, "Name asc"),
            CancellationToken.None);

        List<TenantResponse> items = await ReadItemsAsync(response.Items);

        Assert.Equal(2, response.TotalCount);
        Assert.Single(items);
        Assert.Equal(alpha.Id, items[0].Id);
        Assert.Equal("Alpha Tenant", items[0].Nome);
    }

    [Fact(DisplayName = "Should include deleted tenants when the deleted filter is null")]
    public async Task GivenDeletedFilterNull_WhenHandled_ThenReturnDeletedTenantsToo()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();

        await AdministrationTestData.AddTenantAsync(dbContext, name: "Alpha Tenant");
        await AdministrationTestData.AddTenantAsync(dbContext, name: "Beta Tenant");
        await AdministrationTestData.AddTenantAsync(dbContext, name: "Deleted Tenant", isDeleted: true);

        SearchTenantHandler handler = new SearchTenantHandler(
            context.AdministrationDbContextFactory,
            new SearchTenantValidator());

        CollectionResponse<TenantResponse> response = await handler.Handle(
            new SearchTenantQuery(null, 10, 0, "Name asc", null),
            CancellationToken.None);

        List<TenantResponse> items = await ReadItemsAsync(response.Items);

        Assert.Equal(3, response.TotalCount);
        Assert.Equal(
            ["Alpha Tenant", "Beta Tenant", "Deleted Tenant"],
            items.Select(item => item.Nome).ToList());
    }

    [Theory(DisplayName = "Should reject invalid tenant search paging")]
    [InlineData(0, 0)]
    [InlineData(1001, 0)]
    [InlineData(1, -1)]
    public async Task GivenInvalidPaging_WhenHandled_ThenThrowValidationException(int take, int skip)
    {
        using CoreTestContext context = new CoreTestContext();
        SearchTenantHandler handler = new SearchTenantHandler(
            context.AdministrationDbContextFactory,
            new SearchTenantValidator());

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new SearchTenantQuery(null, take, skip, "Name asc"), CancellationToken.None));
    }

    private static async Task<List<TenantResponse>> ReadItemsAsync(IAsyncEnumerable<TenantResponse> items)
    {
        List<TenantResponse> result = new List<TenantResponse>();

        await foreach (TenantResponse item in items)
        {
            result.Add(item);
        }

        return result;
    }
}

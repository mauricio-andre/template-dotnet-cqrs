using CqrsProject.Common.Responses;
using CqrsProject.Core.Identity.Responses;
using CqrsProject.Core.Identity.UseCases.SearchRole;
using CqrsProject.Core.Test.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Identity.UseCases;

public class SearchRoleTest
{
    [Fact(DisplayName = "Should search roles by name and paginate")]
    public async Task GivenRoles_WhenSearchingByName_ThenReturnFilteredPage()
    {
        using CoreTestContext context = new CoreTestContext();
        await IdentityTestData.AddRoleAsync(context, "Admin");
        await IdentityTestData.AddRoleAsync(context, "Manager");
        await IdentityTestData.AddRoleAsync(context, "Viewer");

        SearchRoleHandler handler = new SearchRoleHandler(
            new SearchRoleValidator(),
            context.RoleManager);

        CollectionResponse<RoleResponse> response = await handler.Handle(
            new SearchRoleQuery("view", 1, 0, "Name asc"),
            CancellationToken.None);

        List<RoleResponse> items = await ReadItemsAsync(response.Items);

        Assert.Equal(1, response.TotalCount);
        Assert.Single(items);
        Assert.Equal("Viewer", items[0].Name);
    }

    private static async Task<List<RoleResponse>> ReadItemsAsync(IAsyncEnumerable<RoleResponse> items)
    {
        List<RoleResponse> result = new List<RoleResponse>();
        await foreach (RoleResponse item in items)
        {
            result.Add(item);
        }

        return result;
    }
}

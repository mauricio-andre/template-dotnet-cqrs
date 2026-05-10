using CqrsProject.Common.Responses;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Identity.Responses;
using CqrsProject.Core.Identity.UseCases.SearchUserRole;
using CqrsProject.Core.Test.Identity;
using CqrsProject.Core.Test.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Identity.UseCases;

public class SearchUserRoleTest
{
    [Fact(DisplayName = "Should search user roles by user and role names")]
    public async Task GivenUserRoles_WhenSearching_ThenReturnFilteredItems()
    {
        using CoreTestContext context = new CoreTestContext();
        var user = await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com");
        var role = await IdentityTestData.AddRoleAsync(context, "Admin");
        await IdentityTestData.AddUserToRoleAsync(context, user, role);

        SearchUserRoleHandler handler = new SearchUserRoleHandler(
            context.AdministrationDbContextFactory,
            new SearchUserRoleValidator());

        CollectionResponse<SearchUserRoleResponse> response = await handler.Handle(
            new SearchUserRoleQuery(user.Id, role.Id, "ali", "adm", 10, 0, null),
            CancellationToken.None);

        List<SearchUserRoleResponse> items = await ReadItemsAsync(response.Items);

        Assert.Equal(1, response.TotalCount);
        Assert.Single(items);
        Assert.Equal(user.Id, items[0].UserId);
        Assert.Equal(role.Id, items[0].RoleId);
    }

    private static async Task<List<SearchUserRoleResponse>> ReadItemsAsync(IAsyncEnumerable<SearchUserRoleResponse> items)
    {
        List<SearchUserRoleResponse> result = new List<SearchUserRoleResponse>();
        await foreach (SearchUserRoleResponse item in items)
        {
            result.Add(item);
        }

        return result;
    }
}


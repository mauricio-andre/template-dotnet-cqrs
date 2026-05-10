using CqrsProject.Common.Responses;
using CqrsProject.Core.Identity.UseCases.SearchRoleClaim;
using CqrsProject.Core.Test.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Identity.UseCases;

public class SearchRoleClaimTest
{
    [Fact(DisplayName = "Should search claims belonging to a role")]
    public async Task GivenRoleClaims_WhenSearching_ThenReturnMatchingClaims()
    {
        using CoreTestContext context = new CoreTestContext();
        var role = await IdentityTestData.AddRoleAsync(context, "Admin");
        await IdentityTestData.AddRoleClaimAsync(context, role, "permission", "read");
        await IdentityTestData.AddRoleClaimAsync(context, role, "scope", "write");

        SearchRoleClaimHandler handler = new SearchRoleClaimHandler(
            context.AdministrationDbContextFactory,
            new SearchRoleClaimValidator());

        CollectionResponse<KeyValuePair<string, string>> response = await handler.Handle(
            new SearchRoleClaimQuery(role.Id, "per", 10, 0, "ClaimType asc"),
            CancellationToken.None);

        List<KeyValuePair<string, string>> items = await ReadItemsAsync(response.Items);

        Assert.Equal(1, response.TotalCount);
        Assert.Single(items);
        Assert.Equal("permission", items[0].Key);
        Assert.Equal("read", items[0].Value);
    }

    private static async Task<List<KeyValuePair<string, string>>> ReadItemsAsync(IAsyncEnumerable<KeyValuePair<string, string>> items)
    {
        List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
        await foreach (KeyValuePair<string, string> item in items)
        {
            result.Add(item);
        }

        return result;
    }
}

using CqrsProject.Common.Responses;
using CqrsProject.Core.Identity.Responses;
using CqrsProject.Core.Identity.UseCases.SearchUser;
using CqrsProject.Core.Test.Identity;
using CqrsProject.Core.Test.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Identity.UseCases;

public class SearchUserTest
{
    [Fact(DisplayName = "Should search users by term and paginate")]
    public async Task GivenUsers_WhenSearchingByTerm_ThenReturnFilteredPage()
    {
        using CoreTestContext context = new CoreTestContext();
        await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com");
        await IdentityTestData.AddUserAsync(context, userName: "bob", email: "bob@example.com", isDeleted: true);
        await IdentityTestData.AddUserAsync(context, userName: "alina", email: "alina@example.com");

        SearchUserHandler handler = new SearchUserHandler(
            new SearchUserValidator(),
            context.UserManager);

        CollectionResponse<UserResponse> response = await handler.Handle(
            new SearchUserQuery("ali", null, 1, 0, "UserName asc"),
            CancellationToken.None);

        List<UserResponse> items = await ReadItemsAsync(response.Items);

        Assert.Equal(2, response.TotalCount);
        Assert.Single(items);
        Assert.Equal("alice", items[0].UserName);
    }

    [Fact(DisplayName = "Should reject invalid paging parameters when searching users")]
    public async Task GivenInvalidPaging_WhenHandled_ThenThrowValidationException()
    {
        using CoreTestContext context = new CoreTestContext();
        SearchUserHandler handler = new SearchUserHandler(
            new SearchUserValidator(),
            context.UserManager);

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new SearchUserQuery(null, null, 0, 0, null), CancellationToken.None));
    }

    private static async Task<List<UserResponse>> ReadItemsAsync(IAsyncEnumerable<UserResponse> items)
    {
        List<UserResponse> result = new List<UserResponse>();
        await foreach (UserResponse item in items)
        {
            result.Add(item);
        }

        return result;
    }
}


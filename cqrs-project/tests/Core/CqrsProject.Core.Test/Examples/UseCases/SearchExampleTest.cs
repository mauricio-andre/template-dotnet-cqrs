using CqrsProject.Common.Responses;
using CqrsProject.Core.Data;
using CqrsProject.Core.Examples.Entities;
using CqrsProject.Core.Examples.Responses;
using CqrsProject.Core.Examples.UseCases.SearchExample;
using CqrsProject.Core.Test.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Examples.UseCases;

public class SearchExampleTest
{
    [Fact(DisplayName = "Should return examples filtered by term")]
    public async Task GivenExamples_WhenSearchingByTerm_ThenReturnFilteredItems()
    {
        using var context = new CoreSqliteTestContext();
        await AddExamplesAsync(context.DbContext, "Alpha", "Beta", "Alpine");
        var handler = new SearchExampleHandler(
            context.DbContext,
            new SearchExampleValidator());

        var response = await handler.Handle(
            new SearchExampleQuery("alp", 50, 0, "Name asc"),
            CancellationToken.None);

        Assert.Equal(2, response.TotalCount);
        Assert.Equal(
            ["Alpha", "Alpine"],
            await ReadNamesAsync(response.Items));
    }

    [Fact(DisplayName = "Should apply sorting and pagination to the result set")]
    public async Task GivenExamples_WhenSortingAndPaging_ThenReturnRequestedPage()
    {
        using var context = new CoreSqliteTestContext();
        await AddExamplesAsync(context.DbContext, "Gamma", "Alpha", "Beta");
        var handler = new SearchExampleHandler(
            context.DbContext,
            new SearchExampleValidator());

        var response = await handler.Handle(
            new SearchExampleQuery(null, 2, 1, "Name desc"),
            CancellationToken.None);

        Assert.Equal(3, response.TotalCount);
        Assert.Equal(
            ["Beta", "Alpha"],
            await ReadNamesAsync(response.Items));
    }

    [Theory(DisplayName = "Should reject invalid paging values")]
    [InlineData(0, 0)]
    [InlineData(1001, 0)]
    [InlineData(1, -1)]
    public async Task GivenInvalidPaging_WhenHandled_ThenThrowValidationException(int take, int skip)
    {
        using var context = new CoreSqliteTestContext();
        var handler = new SearchExampleHandler(
            context.DbContext,
            new SearchExampleValidator());

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new SearchExampleQuery(null, take, skip, "Name asc"), CancellationToken.None));
    }

    private static async Task AddExamplesAsync(CoreDbContext dbContext, params string[] names)
    {
        foreach (var name in names)
            dbContext.Examples.Add(new Example { Name = name });

        await dbContext.SaveChangesAsync();
    }

    private static async Task<List<string>> ReadNamesAsync(IAsyncEnumerable<ExampleResponse> items)
    {
        var names = new List<string>();

        await foreach (var item in items)
            names.Add(item.Name);

        return names;
    }
}

using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Data;
using CqrsProject.Core.Examples.Entities;
using CqrsProject.Core.Examples.UseCases.GetExampleByKey;
using CqrsProject.Core.Test.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Examples.UseCases;

public class GetExampleByKeyTest
{
    [Fact(DisplayName = "Should return the example when the key exists")]
    public async Task GivenExistingExample_WhenHandled_ThenReturnExample()
    {
        using var context = new CoreSqliteTestContext();
        var example = await AddExampleAsync(context.DbContext, "Example One");
        var handler = new GetExampleByKeyHandler(
            context.DbContext,
            new GetExampleByKeyValidator(),
            context.Localizer);

        var response = await handler.Handle(new GetExampleByKeyQuery(example.Id), CancellationToken.None);

        Assert.Equal(example.Id, response.Id);
        Assert.Equal("Example One", response.Name);
    }

    [Fact(DisplayName = "Should fail when the example key does not exist")]
    public async Task GivenMissingExample_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using var context = new CoreSqliteTestContext();
        var handler = new GetExampleByKeyHandler(
            context.DbContext,
            new GetExampleByKeyValidator(),
            context.Localizer);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new GetExampleByKeyQuery(999), CancellationToken.None));
    }

    private static async Task<Example> AddExampleAsync(CoreDbContext dbContext, string name)
    {
        var example = new Example { Name = name };
        dbContext.Examples.Add(example);
        await dbContext.SaveChangesAsync();
        return example;
    }
}

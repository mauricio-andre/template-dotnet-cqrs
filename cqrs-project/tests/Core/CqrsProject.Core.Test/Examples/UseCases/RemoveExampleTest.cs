using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Data;
using CqrsProject.Core.Examples.Entities;
using CqrsProject.Core.Examples.UseCases.RemoveExample;
using CqrsProject.Core.Test.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Examples.UseCases;

public class RemoveExampleTest
{
    [Fact(DisplayName = "Should remove the example when the key exists")]
    public async Task GivenExistingExample_WhenHandled_ThenRemoveExample()
    {
        using var context = new CoreSqliteTestContext();
        var example = await AddExampleAsync(context.DbContext, "Example One");
        var handler = new RemoveExampleHandler(
            context.DbContext,
            new RemoveExampleValidator(),
            context.Localizer);

        await handler.Handle(new RemoveExampleCommand(example.Id), CancellationToken.None);

        Assert.Equal(0, await context.DbContext.Examples.CountAsync());
    }

    [Fact(DisplayName = "Should fail when removing a missing example")]
    public async Task GivenMissingExample_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using var context = new CoreSqliteTestContext();
        var handler = new RemoveExampleHandler(
            context.DbContext,
            new RemoveExampleValidator(),
            context.Localizer);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new RemoveExampleCommand(999), CancellationToken.None));
    }

    private static async Task<Example> AddExampleAsync(CoreDbContext dbContext, string name)
    {
        var example = new Example { Name = name };
        dbContext.Examples.Add(example);
        await dbContext.SaveChangesAsync();
        return example;
    }
}

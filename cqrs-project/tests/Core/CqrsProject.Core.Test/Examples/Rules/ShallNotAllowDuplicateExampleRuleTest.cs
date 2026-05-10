using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Examples.Entities;
using CqrsProject.Core.Examples.Rules;
using CqrsProject.Core.Examples.UseCases.CreateExample;
using CqrsProject.Core.Test.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Examples.Rules;

public class ShallNotAllowDuplicateExampleRuleTest
{
    [Fact(DisplayName = "Should allow creating an example when no duplicate exists")]
    public async Task GivenUniqueExampleName_WhenHandled_ThenCompleteWithoutException()
    {
        using var context = new CoreTestContext();
        using var dbContext = context.CreateCoreDbContext();
        await dbContext.Examples.AddAsync(new Example { Name = "Example One" });
        await dbContext.SaveChangesAsync();
        var rule = new ShallNotAllowDuplicateExampleRule(dbContext, context.Localizer);

        await rule.Handle(new CreateExampleEvent("Example Two"), CancellationToken.None);

        Assert.Equal(1, await dbContext.Examples.CountAsync());
    }

    [Fact(DisplayName = "Should reject duplicated example names regardless of casing")]
    public async Task GivenDuplicatedExampleName_WhenHandled_ThenThrowDuplicatedEntityException()
    {
        using var context = new CoreTestContext();
        using var dbContext = context.CreateCoreDbContext();
        await dbContext.Examples.AddAsync(new Example { Name = "Example One" });
        await dbContext.SaveChangesAsync();

        context.Localizer.Set("message:validation:duplicatedEntity", "Duplicated {0}");
        context.Localizer.Set("message:validation:valueAlreadyUse", "The value {0} is already in use");

        var rule = new ShallNotAllowDuplicateExampleRule(dbContext, context.Localizer);

        var exception = await Assert.ThrowsAsync<DuplicatedEntityException>(
            () => rule.Handle(new CreateExampleEvent("example one"), CancellationToken.None));

        Assert.Equal("Duplicated Example", exception.Message);
        Assert.True(exception.Errors.ContainsKey(nameof(Example.Name)));
        Assert.Equal("The value example one is already in use", exception.Errors[nameof(Example.Name)].Single());
    }
}

using CqrsProject.Core.Data;
using CqrsProject.Core.Examples.Entities;
using CqrsProject.Core.Examples.UseCases.CreateExample;
using CqrsProject.Core.Test.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace CqrsProject.Core.Test.Examples.UseCases;

public class CreateExampleTest
{
    [Fact(DisplayName = "Should create an example and publish the creation event")]
    public async Task GivenValidCommand_WhenHandled_ThenPersistExampleAndPublishEvent()
    {
        using var context = new CoreSqliteTestContext();
        var handler = new CreateExampleHandler(
            context.DbContext,
            new CreateExampleValidator(),
            context.Mediator);

        var response = await handler.Handle(new CreateExampleCommand("Example One"), CancellationToken.None);

        Assert.Equal("Example One", response.Name);
        Assert.True(response.Id > 0);
        Assert.Equal(1, await context.DbContext.Examples.CountAsync());
        Assert.Equal("Example One", await context.DbContext.Examples.Select(example => example.Name).SingleAsync());

        await context.Mediator.Received(1).Publish(
            Arg.Is<CreateExampleEvent>(notification => notification.Name == "Example One"),
            Arg.Any<CancellationToken>());
    }

    [Theory(DisplayName = "Should reject invalid example names")]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GivenInvalidName_WhenHandled_ThenThrowValidationException(string name)
    {
        using var context = new CoreSqliteTestContext();
        var handler = new CreateExampleHandler(
            context.DbContext,
            new CreateExampleValidator(),
            context.Mediator);

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new CreateExampleCommand(name), CancellationToken.None));

        Assert.Equal(0, await context.DbContext.Examples.CountAsync());
    }

    [Fact(DisplayName = "Should reject names above the maximum length")]
    public async Task GivenNameAboveMaxLength_WhenHandled_ThenThrowValidationException()
    {
        using var context = new CoreSqliteTestContext();
        var handler = new CreateExampleHandler(
            context.DbContext,
            new CreateExampleValidator(),
            context.Mediator);

        var name = new string('a', ExampleConstrains.NameMaxLength + 1);

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new CreateExampleCommand(name), CancellationToken.None));

        Assert.Equal(0, await context.DbContext.Examples.CountAsync());
    }
}

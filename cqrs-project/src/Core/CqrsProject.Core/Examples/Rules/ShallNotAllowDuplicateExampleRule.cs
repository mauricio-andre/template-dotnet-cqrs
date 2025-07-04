using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Examples.Entities;
using CqrsProject.Core.Examples.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Examples.Rules;

public class ShallNotAllowDuplicateExampleRule : INotificationHandler<CreateExampleEvent>
{
    private readonly CoreDbContext _coreDbContext;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public ShallNotAllowDuplicateExampleRule(CoreDbContext coreDbContext, IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _coreDbContext = coreDbContext;
        _stringLocalizer = stringLocalizer;
    }

    public async Task Handle(
        CreateExampleEvent notification,
        CancellationToken cancellationToken)
    {
        var hasDuplicate = await _coreDbContext.Examples
            .AnyAsync(example => example.Name.ToLower() == notification.Name.ToLower());

        if (hasDuplicate)
            throw new DuplicatedEntityException(
                _stringLocalizer,
                nameof(Example),
                new Dictionary<string, string[]>
                {
                    {
                        nameof(Example.Name),
                        [ _stringLocalizer["message:validation:valueAlreadyUse", notification.Name] ]
                    }
                });
    }
}

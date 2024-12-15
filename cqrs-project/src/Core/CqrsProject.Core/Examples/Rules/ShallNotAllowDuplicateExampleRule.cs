using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Rules;

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
            throw new DuplicatedEntityException(_stringLocalizer, nameof(Examples));
    }
}

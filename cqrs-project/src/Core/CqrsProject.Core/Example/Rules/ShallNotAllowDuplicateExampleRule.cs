using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Events;
using CqrsProject.Core.Exceptions;
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
            .AnyAsync(example => example.Name.Equals(notification.name));

        if (hasDuplicate)
            throw new DuplicatedExampleException(_stringLocalizer);
    }
}

using CqrsProject.Core.Data;
using CqrsProject.Core.Events;
using CqrsProject.Core.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Rules;

public class ShallNotAllowDuplicateExampleRule : INotificationHandler<CreateExampleEvent>
{
    private readonly CoreDbContext _coreDbContext;

    public ShallNotAllowDuplicateExampleRule(CoreDbContext coreDbContext) => _coreDbContext = coreDbContext;

    public async Task Handle(
        CreateExampleEvent notification,
        CancellationToken cancellationToken)
    {
        var hasDuplicate = await _coreDbContext.Examples
            .AnyAsync(example => example.Name.Equals(notification.name));

        // TODO: Colocar mensagem no sistema multi language
        if (hasDuplicate)
            throw new DuplicatedExampleException("Mensagem a ser configurada");
    }
}

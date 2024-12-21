using CqrsProject.Core.Data;
using CqrsProject.Core.Examples.Commands;
using CqrsProject.Core.Examples.Entities;
using CqrsProject.Core.Examples.Events;
using CqrsProject.Core.Examples.Responses;
using FluentValidation;
using MediatR;

namespace CqrsProject.Core.Examples.Handlers;

public class CreateExampleHandler : IRequestHandler<CreateExampleCommand, ExampleResponse>
{
    private readonly CoreDbContext _coreDbContext;
    private readonly IValidator<CreateExampleCommand> _validator;
    private readonly IMediator _mediator;

    public CreateExampleHandler(
        CoreDbContext coreDbContext,
        IValidator<CreateExampleCommand> validator,
        IMediator mediator)
    {
        _coreDbContext = coreDbContext;
        _validator = validator;
        _mediator = mediator;
    }

    public async Task<ExampleResponse> Handle(
        CreateExampleCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        await _mediator.Publish(new CreateExampleEvent(request.Name));
        var entity = MapToEntity(request);
        _coreDbContext.Add(entity);
        await _coreDbContext.SaveChangesAsync(cancellationToken);
        return MapToResponse(entity);
    }

    private static Example MapToEntity(CreateExampleCommand request)
        => new Example { Name = request.Name };

    private static ExampleResponse MapToResponse(Example entity)
        => new ExampleResponse(entity.Id, entity.Name);
}

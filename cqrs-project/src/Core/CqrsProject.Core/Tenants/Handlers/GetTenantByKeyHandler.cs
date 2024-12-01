using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Queries;
using CqrsProject.Core.Responses;
using CqrsProject.Core.Tenants;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Handlers;

public class GetTenantByKeyHandler : IRequestHandler<GetTenantByKeyQuery, TenantResponse>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IValidator<GetTenantByKeyQuery> _validator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public GetTenantByKeyHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<GetTenantByKeyQuery> validator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _validator = validator;
        _stringLocalizer = stringLocalizer;
    }

    public async Task<TenantResponse> Handle(
        GetTenantByKeyQuery request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var entity = await _administrationDbContext.Tenants.FirstOrDefaultAsync(
            tenant => tenant.Id == request.Id,
            cancellationToken);

        if (entity == null)
            throw new EntityNotFoundException(_stringLocalizer, nameof(Tenant), request.Id.ToString());

        return MapToResponse(entity);
    }

    private static TenantResponse MapToResponse(Tenant entity)
        => new TenantResponse(entity.Id, entity.Name, entity.IsDeleted);
}

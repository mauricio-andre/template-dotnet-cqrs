using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using CqrsProject.Core.UserTenants.Queries;
using CqrsProject.Common.Consts;
using CqrsProject.Core.Identity.Commands;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CqrsProject.App.GrpcServer.Methods.V1.Me;

[Authorize]
public class MeGrpcService : MeService.MeServiceBase
{
    private readonly IMediator _mediator;
    public MeGrpcService(IMediator mediator) => _mediator = mediator;

    public override async Task<Empty> SyncUser(Empty request, ServerCallContext context)
    {
        var httpContext = context.GetHttpContext();

        await _mediator.Send(new IdentitySyncCommand(
            NameIdentifier: httpContext.User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)!.Value,
            AccessToken: httpContext.Request.Headers.Authorization.ToString().Split(" ")[1]));

        return new Empty();
    }

    public async override Task ListTenants(
        Empty request,
        IServerStreamWriter<MeTenantReply> responseStream,
        ServerCallContext context)
    {
        var total = 0;
        var skip = 0;
        var take = 500;

        do
        {
            var result = await _mediator.Send(new SearchMeTenantQuery(null, null, take, skip, null));
            total = result.TotalCount;
            skip += take;

            await foreach (var item in result.Items)
            {
                await responseStream.WriteAsync(new MeTenantReply
                {
                    Id = item.Id.ToString(),
                    TenantName = item.TenantName
                });
            }
        } while (skip < total);
    }

    public override async Task ListPermissions(
        Empty request,
        IServerStreamWriter<MePermissionReply> responseStream,
        ServerCallContext context)
    {
        var list = context.GetHttpContext().User.Identities
            .SelectMany(identity => identity.Claims)
            .Where(claim => claim.Type == AuthorizationPermissionClaims.ClaimType)
            .Select(claim => claim.Value)
            .Order();

        foreach (var item in list)
            await responseStream.WriteAsync(new MePermissionReply
            {
                Permission = item
            });
    }

}

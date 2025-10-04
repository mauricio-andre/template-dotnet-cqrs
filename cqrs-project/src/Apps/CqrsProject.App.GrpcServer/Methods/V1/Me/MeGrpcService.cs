using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using CqrsProject.Core.UserTenants.Queries;
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
        int total;
        var skip = 0;
        var take = 1000;

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
}

using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using CqrsProject.Core.UserTenants.Queries;

namespace CqrsProject.App.GrpcServer.V1.Me.Services;

// [Authorize]
public class MeService : MeServiceGrpc.MeServiceGrpcBase
{
    private readonly IMediator _mediator;
    public MeService(IMediator mediator) => _mediator = mediator;

    public async override Task GetTenants(
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
}

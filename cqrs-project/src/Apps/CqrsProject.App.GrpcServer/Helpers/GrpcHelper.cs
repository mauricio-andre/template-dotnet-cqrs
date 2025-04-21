using Grpc.Core;

namespace CqrsProject.App.GrpcServer.Helpers;

public static class GrpcHelper
{
    public static void CheckRequireHeaderTenantId(ServerCallContext context)
    {
        var httpContext = context.GetHttpContext();
        if (httpContext.Request.Headers.TryGetValue("tenant-id", out var _))
            return;

        throw new RpcException(new Status(StatusCode.FailedPrecondition, "tenant-id is missing"));
    }
}

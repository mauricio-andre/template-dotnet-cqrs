using Grpc.Core;

namespace CqrsProject.App.GrpcServer.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireTenantIdInterceptorAttribute : InterceptorAttribute
{
    public RequireTenantIdInterceptorAttribute()
    {
    }

    public override Task<TResponse> OnActionExecutingUnary<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
        where TRequest : class where TResponse : class
    {
        ThrowExceptionIfHeaderMissing(context);
        return continuation(request, context);
    }

    public override Task OnActionExecutingServerStreaming<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
        where TRequest : class where TResponse : class
    {
        ThrowExceptionIfHeaderMissing(context);
        return continuation(request, responseStream, context);
    }

    public override Task<TResponse> OnActionExecutingClientStreaming<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
        where TRequest : class where TResponse : class
    {
        ThrowExceptionIfHeaderMissing(context);
        return continuation(requestStream, context);
    }

    public override Task OnActionExecutingDuplexStreaming<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        where TRequest : class where TResponse : class
    {
        ThrowExceptionIfHeaderMissing(context);
        return continuation(requestStream, responseStream, context);
    }

    private static void ThrowExceptionIfHeaderMissing(ServerCallContext context)
    {
        if (context.RequestHeaders.GetValue("tenant-id") == null)
            throw new RpcException(
                new Status(
                    StatusCode.FailedPrecondition,
                    "Tenant-Id is missing"));
    }
}

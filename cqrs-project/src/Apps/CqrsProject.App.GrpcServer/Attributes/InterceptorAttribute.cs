using Grpc.Core;

namespace CqrsProject.App.GrpcServer.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public abstract class InterceptorAttribute : Attribute
{
    public virtual Task<TResponse> OnActionExecutingUnary<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
        where TRequest : class where TResponse : class
    {
        return continuation(request, context);
    }

    public virtual Task OnActionExecutingServerStreaming<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
        where TRequest : class where TResponse : class
    {
        return continuation(request, responseStream, context);
    }

    public virtual Task<TResponse> OnActionExecutingClientStreaming<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
        where TRequest : class where TResponse : class
    {
        return continuation(requestStream, context);
    }

    public virtual Task OnActionExecutingDuplexStreaming<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        where TRequest : class where TResponse : class
    {
        return continuation(requestStream, responseStream, context);
    }
}

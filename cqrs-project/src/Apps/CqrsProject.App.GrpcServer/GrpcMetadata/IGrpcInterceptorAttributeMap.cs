using CqrsProject.App.GrpcServer.Attributes;

namespace CqrsProject.App.GrpcServer.GrpcMetadata;

public interface IGrpcInterceptorAttributeMap
{
    public bool TryGetInterceptorsByMethod(
        string method,
        out IEnumerable<InterceptorAttribute>? interceptorAttributes);
}

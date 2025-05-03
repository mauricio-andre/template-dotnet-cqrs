using Grpc.Core;
using Grpc.Reflection.V1Alpha;

namespace CqrsProject.App.GrpcServer.Extensions;

public static class ServerCallContextExtensions
{
    public static bool IsServerReflectionMethod(this ServerCallContext context)
    {
        var nameMethod = $"/{typeof(ServerReflection).Namespace}.{nameof(ServerReflection)}/{nameof(ServerReflection.ServerReflectionBase.ServerReflectionInfo)}";
        return context.Method.Equals(nameMethod, StringComparison.InvariantCultureIgnoreCase);
    }
}

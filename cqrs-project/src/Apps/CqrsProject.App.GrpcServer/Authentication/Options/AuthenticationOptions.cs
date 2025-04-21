using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace CqrsProject.App.GrpcServer.Authentication;

public class AuthenticationOptions : AuthenticationSchemeOptions
{
    public string JwtTokenScheme { get; set; } = JwtBearerDefaults.AuthenticationScheme;
}

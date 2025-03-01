using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CqrsProject.Commons.Test.Helpers;

public static class JwtHelper
{
    public static Dictionary<string, string?> Options =>
        new Dictionary<string, string?>
        {
            { "Authentication:Bearer:Authority", "mySelf" },
            { "Authentication:Bearer:Audience", "myTest" }
        };

    public static string GenerateJwtToken(string userIdentifier)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("chave-secreta-mock-chave-secreta-mock-chave-secreta-mock"));
        securityKey.KeyId = Guid.NewGuid().ToString();
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userIdentifier),
                new Claim("scope", "openid email offline_access"),
            }),
            Expires = DateTime.UtcNow.AddMinutes(30),
            Issuer = JwtHelper.Options["Authentication:Bearer:Authority"],
            Audience = JwtHelper.Options["Authentication:Bearer:Audience"],
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

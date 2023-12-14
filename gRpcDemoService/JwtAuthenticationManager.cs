using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GrpcDemoService;

/// <summary>
/// ------ 預計正式版轉成：不公開的 Singleton injectable service
/// </summary>
static class JwtAuthenticationManager
{
  //const string JWT_TOKEN_KEY = @"Show Me The Money @2023";
  const int JWT_TOKEN_VALIDITY_MINUTES = 30;

  public static string JWT_TOKEN_KEY => @"Show Me The Money @2023 Show Me The Money";

  public static AuthenticationReply? Authenticate(AuthenticationRequest req)
  {
    //## Implement User Credentials Validation --------------------------------
    if (req.UserName != "admin" || req.UserPassword != "admin")
      return null;

    //-------------------------------------------------------------------------
    var tokenKey = Encoding.ASCII.GetBytes(JWT_TOKEN_KEY);
    var tokenExpiryUtc = DateTime.Now.AddMinutes(JWT_TOKEN_VALIDITY_MINUTES).ToUniversalTime();

    var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

    var securityToken = jwtSecurityTokenHandler.CreateToken(new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(new[]
      {
        new Claim(ClaimTypes.Name, req.UserName),
        new Claim(ClaimTypes.Role, "Admin")
      }),
      Expires = tokenExpiryUtc,
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
    });

    var token = jwtSecurityTokenHandler.WriteToken(securityToken);

    return new AuthenticationReply
    {
      AccessToken = token,
      ExpiresIn = (int)tokenExpiryUtc.Subtract(DateTime.UtcNow).TotalSeconds
    };
  }
}

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
  const int JWT_TOKEN_VALIDITY_MINUTES = 1;
  public static string JWT_TOKEN_KEY => @"12345678901234567890123456789012"; // 32 密碼長度需與演算法匹配。
  public static string JWE_SECRET_KEY => @"12345678901234567890123456789012"; // 32 密碼長度需與演算法匹配。
  public static AuthenticationReply? Authenticate(AuthenticationRequest req)
  {
    //## Implement User Credentials Validation --------------------------------
    if (req.UserName != "admin" || req.UserPassword != "admin")
      return null;

    //-------------------------------------------------------------------------
    var tokenKey = Encoding.ASCII.GetBytes(JWT_TOKEN_KEY);
    var secretKey = Encoding.ASCII.GetBytes(JWE_SECRET_KEY);
    var tokenExpiryUtc = DateTime.UtcNow.AddMinutes(JWT_TOKEN_VALIDITY_MINUTES);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(new[]
      {
        new Claim(ClaimTypes.Name, req.UserName),
        new Claim(ClaimTypes.Role, "Admin"),
        new Claim(ClaimTypes.GivenName, "系統管理者"),
      }),
      Expires = tokenExpiryUtc,
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature),
      EncryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.Aes256KW, SecurityAlgorithms.Aes256CbcHmacSha512) // JWE
    };

    var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
    var securityToken = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
    var token = jwtSecurityTokenHandler.WriteToken(securityToken);

    return new AuthenticationReply
    {
      AccessToken = token,
      ExpiresIn = (int)tokenExpiryUtc.Subtract(DateTime.UtcNow).TotalSeconds
    };
  }
}

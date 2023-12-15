using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace GrpcDemoService.Services;

public class AuthenticationService : Authentication.AuthenticationBase
{
  public override Task<AuthenticationReply> Authenticate(AuthenticationRequest req, ServerCallContext ctx)
  {
    try
    {
      var reply = JwtAuthenticationManager.Authenticate(req);
      if (reply == null)
        throw new RpcException(new Status(StatusCode.Unauthenticated, "登入認證失敗！"));

      return Task.FromResult(reply);
    }
    catch(RpcException)
    {
      throw;
    }
    catch (Exception ex)
    {
      /// 把 Exception 轉換成 RpcException 以識別錯誤訊息。
      /// 否則還是會自動轉成 RpcException(Unkonw) 這樣無法識別錯誤訊息。 => Status(StatusCode="Unknown", Detail="Exception was thrown by handler.")
      throw new RpcException(new Status(StatusCode.Unauthenticated, ex.Message, ex));
    }
  }

  //[Authorize]
  public override Task<AuthStateReply> GetAuthState(Empty req, ServerCallContext ctx)
  {
    try
    {
      HttpContext httpCtx = ctx.GetHttpContext();
      
      if(!(httpCtx.User.Identity?.IsAuthenticated ?? false))
      {
        // 來賓
        return Task.FromResult(new AuthStateReply
        {
          UserId = "guest",
          UserName = "來賓",
          Roles = "Guest",
          IssuedAtUtc = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow),
          NotBeforeUtc = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow),
          ExpiresUtc = Timestamp.FromDateTimeOffset(DateTimeOffset.MaxValue),
        });
      }

      //## 開始解析 Token 內容
      string userId = httpCtx.User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
      string userName = httpCtx.User.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty;
      string roles = String.Join(",", httpCtx.User.FindAll(ClaimTypes.Role).Select(x => x.Value));

      var issuedAt = httpCtx.User.FindFirst(JwtRegisteredClaimNames.Iat);
      DateTimeOffset issuedAtUtc = DateTimeOffset.FromUnixTimeSeconds(long.Parse(issuedAt!.Value));

      var notBefore = httpCtx.User.FindFirst(JwtRegisteredClaimNames.Nbf);
      DateTimeOffset notBeforeUtc = DateTimeOffset.FromUnixTimeSeconds(long.Parse(notBefore!.Value));

      var expires = httpCtx.User.FindFirst(JwtRegisteredClaimNames.Exp);
      DateTimeOffset expiresUtc = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expires!.Value));

      return Task.FromResult(new AuthStateReply{
        UserId = userId,
        UserName = userName,
        Roles = roles,
        IssuedAtUtc = Timestamp.FromDateTimeOffset(issuedAtUtc),
        NotBeforeUtc = Timestamp.FromDateTimeOffset(notBeforeUtc),
        ExpiresUtc = Timestamp.FromDateTimeOffset(expiresUtc),
      });
    }
    catch (Exception ex)
    {
      /// 把 Exception 轉換成 RpcException 以識別錯誤訊息。
      /// 否則還是會自動轉成 RpcException(Unkonw) 這樣無法識別錯誤訊息。 => Status(StatusCode="Unknown", Detail="Exception was thrown by handler.")
      throw new RpcException(new Status(StatusCode.Aborted, ex.Message, ex));
    }
  }
}

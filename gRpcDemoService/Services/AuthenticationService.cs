using Grpc.Core;

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
}

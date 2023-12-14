using Grpc.Core;

namespace GrpcDemoService.Services;

public class AuthenticationService : Authentication.AuthenticationBase
{
  public override Task<AuthenticationReply> Authenticate(AuthenticationRequest req, ServerCallContext ctx)
  {
    var reply = JwtAuthenticationManager.Authenticate(req);
    if (reply == null)
      throw new RpcException(new Status(StatusCode.Unauthenticated, "登入認證失敗！"));

    return Task.FromResult(reply);
  }
}

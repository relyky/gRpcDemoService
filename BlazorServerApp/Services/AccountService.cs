using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using GrpcDemoService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorServerApp.Services;

public interface ITokenProvider
{
  Task<string> GetTokenAsync(CancellationToken cancellationToken);
}

internal class AccountService(
  IMemoryCache _cache,
  ILoggerFactory _loggerFactory,
  IConfiguration _config)
  : ITokenProvider
{
  //## Resource
  readonly string gRPCHostAddress = _config["gRPCHostAddress"];

  public async Task<AuthenticationReply> AuthenticateAsync(AuthenticationRequest req)
  {
    // 全手動叫用 Grpc Cllient。
    using var channel = GrpcChannel.ForAddress(gRPCHostAddress);
    var invoker = channel.Intercept(new GrpcLoggerInterceptor(_loggerFactory));
    var client = new Authentication.AuthenticationClient(invoker);

    var headers = new Metadata();
    //headers.Add("X-API-KEY", $"{ApiKey}");

    var reply = await client.AuthenticateAsync(req, headers);

    // 把 AccessToken 存入 Token Storage。
    _cache.Set("AccessToken", reply.AccessToken, DateTimeOffset.UtcNow.AddSeconds(reply.ExpiresIn));
    return reply;
  }

  Task<string> ITokenProvider.GetTokenAsync(CancellationToken cancellationToken)
  {
    if (cancellationToken.IsCancellationRequested)
      throw new TaskCanceledException("已取消登入。");

    string accessToken = _cache.Get<string>("AccessToken");
    return Task.FromResult(accessToken);
  }

  public async Task<AuthStateReply> GetAuthStateAsync()
  {
    // 附上 Access Token
    var headers = new Metadata();
    headers.Add("Authorization", $"Bearer {_cache.Get<string>("AccessToken")}");

    // 全手動叫用 Grpc Cllient。
    using var channel = GrpcChannel.ForAddress(gRPCHostAddress);
    var invoker = channel.Intercept(new GrpcLoggerInterceptor(_loggerFactory));
    var client = new Authentication.AuthenticationClient(invoker);
    var reply = await client.GetAuthStateAsync(new Empty(), headers);
    return reply;
  }


}

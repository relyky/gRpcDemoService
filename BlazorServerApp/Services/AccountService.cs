using Grpc.Core;
using Grpc.Net.Client;
using GrpcDemoService;
using Microsoft.AspNetCore.Authentication;

namespace BlazorServerApp.Services;

internal class AccountService
{
  //## Resource
  string gRPCHostAddress = @"https://localhost:7176"; // http://localhost:5220, 

  // Authenticate
  // Authorize

  public async Task<AuthenticationReply> Authenticate(AuthenticationRequest req)
  {
    using var channel = GrpcChannel.ForAddress(gRPCHostAddress);
    var client = new Authentication.AuthenticationClient(channel);
    var reply = await client.AuthenticateAsync(req);
    return reply;
  }
}

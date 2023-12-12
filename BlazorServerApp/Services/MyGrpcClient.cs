using Grpc.Net.Client;
using GrpcDemoService;

namespace BlazorServerApp.Services;

internal class MyGrpcClient
{
  public async Task<SampleReply> GetFullNameAsync(SampleRequest req)
  {
    try
    {
      using var channel = GrpcChannel.ForAddress("https://localhost:7176"); // http://localhost:5220, 
      var client = new Sample.SampleClient(channel);
      var reply = await client.GetFullNameAsync(req);

      await channel.ShutdownAsync();
      return reply;
    }
    catch(Exception ex) 
    {
      return new SampleReply() { FullName = "出現例外!" + ex.Message };
    }
  }
}

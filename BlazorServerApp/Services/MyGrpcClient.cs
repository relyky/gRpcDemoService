using BlazorServerApp.Models;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcDemoService;

namespace BlazorServerApp.Services;

internal class MyGrpcClient
{
  //## Resource
  string gRPCHostAddress = @"https://localhost:7176"; // http://localhost:5220, 

  readonly ILogger<MyGrpcClient> _logger;
  public MyGrpcClient(ILogger<MyGrpcClient> logger)
  {
    _logger = logger;
  }

  public async Task<HelloReply> HelloAsync(HelloRequest req)
  {
    using var channel = GrpcChannel.ForAddress(gRPCHostAddress);
    var client = new Greeter.GreeterClient(channel);
    var reply = await client.SayHelloAsync(req);
    return reply;
  }

  public async Task<(SampleReply? reply, string errMsg)> GetFullNameAsync(SampleRequest req)
  {
    if(req.FirstName == "logical" && req.LastName == "error")
      return (null, "測試發生邏輯錯誤。");

    using var channel = GrpcChannel.ForAddress(gRPCHostAddress);
    var client = new Sample.SampleClient(channel);
    var reply = await client.GetFullNameAsync(req);

    await channel.ShutdownAsync();
    return (reply, Constant.Success);
  }

  public async Task<ProductSaveReply> SaveProductAsync()
  {
    using var channel = GrpcChannel.ForAddress(gRPCHostAddress);
    var client = new Product.ProductClient(channel);

    var dataModel = new ProductModel
    {
      ProductName = "Macbook Pro",
      ProductCode = "P1001",
      Price = 5000
    };

    var reply = await client.SaveProductAsync(dataModel);
    return reply;
  }
}

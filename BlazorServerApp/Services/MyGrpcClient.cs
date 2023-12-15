using BlazorServerApp.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using GrpcDemoService;
using BlazorServerApp.Services;

namespace BlazorServerApp.Services;

internal class MyGrpcClient(ILoggerFactory loggerFactory)
{
  //## Resource
  string gRPCHostAddress = @"https://localhost:7176"; // http://localhost:5220, 

  public async Task<HelloReply> HelloAsync(HelloRequest req)
  {
    using var channel = GrpcChannel.ForAddress(gRPCHostAddress);
    var client = new Greeter.GreeterClient(channel);
    var reply = await client.SayHelloAsync(req);

    await channel.ShutdownAsync();
    return reply;
  }

  public async Task<(SampleReply? reply, string errMsg)> GetFullNameAsync(SampleRequest req)
  {
    if (req.FirstName == "logical" && req.LastName == "error")
      return (null, "測試發生邏輯錯誤。");

    using var channel = GrpcChannel.ForAddress(gRPCHostAddress);
    var client = new Sample.SampleClient(channel);
    var reply = await client.GetFullNameAsync(req);

    await channel.ShutdownAsync();
    return (reply, Constant.Success);
  }

  /// <summary>
  /// ------ 臨時取名測試 登入機制，正式版建議存在後端，比如：Cache。
  /// </summary>
  public async Task<(SampleReply? reply, string errMsg)> Auth_GetFullNameAsync(SampleRequest req, AuthenticationReply authReply)
  {
    if (req.FirstName == "logical" && req.LastName == "error")
      return (null, "測試發生邏輯錯誤。");

    if (authReply == null)
      return (null, "需先登入認證。");

    // 附上 Access Token
    var headers = new Metadata();
    headers.Add("Authorization", $"Bearer {authReply.AccessToken}");

    using var channel = GrpcChannel.ForAddress(gRPCHostAddress);
    var client = new Sample.SampleClient(channel);
    var reply = await client.GetFullNameAsync(req, headers);

    await channel.ShutdownAsync();
    return (reply, Constant.Success);
  }

  public async Task<ProductSaveReply> SaveProductAsync()
  {
    using var channel = GrpcChannel.ForAddress(gRPCHostAddress);
    var invoker = channel.Intercept(new GrpcLoggerInterceptor(loggerFactory));

    var client = new Product.ProductClient(invoker);
    //var client = new Product.ProductClient(channel);

    //※ gRCP 的日期格式必需是 UTC。
    //Timestamp stockDate = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2023, 12, 13), DateTimeKind.Utc));
    Timestamp stockDate = Timestamp.FromDateTime(new DateTime(2023, 12, 13).ToUniversalTime());
    //Timestamp stockDate = Timestamp.FromDateTimeOffset(DateTimeOffset.Now);

    var dataModel = new ProductModel
    {
      ProductName = "Macbook Pro",
      ProductCode = "P1001",
      Price = 5000,
      StockDate = stockDate
    };

    var reply = await client.SaveProductAsync(dataModel);
    return reply;
  }

  public async Task<ProductListReply> GetProductListAsync()
  {
    using var channel = GrpcChannel.ForAddress(gRPCHostAddress);
    var client = new Product.ProductClient(channel);

    var reply = await client.GetProductsAsync(new Empty(), deadline: DateTime.UtcNow.AddSeconds(3)); // 測試 deadline
    return reply;
  }

  public async Task<AuthStateReply> GetAuthStateAsync(AuthenticationReply authReply)
  {
    // 附上 Access Token
    var headers = new Metadata();
    headers.Add("Authorization", $"Bearer {authReply.AccessToken}");

    using var channel = GrpcChannel.ForAddress(gRPCHostAddress);
    var client = new Authentication.AuthenticationClient(channel);
    var reply = await client.GetAuthStateAsync(new Empty(), headers);
    return reply;
  }
}

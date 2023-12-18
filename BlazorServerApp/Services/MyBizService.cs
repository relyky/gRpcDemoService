using BlazorServerApp.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using GrpcDemoService;
using BlazorServerApp.Services;
using Microsoft.Extensions.Logging;

namespace BlazorServerApp.Services;

internal class MyBizService(
  Product.ProductClient _prodClient,
  Sample.SampleClient _sampleClient,
  IConfiguration _config,
  ILoggerFactory _loggerFactory)
{
  //## Resource
  readonly string gRPCHostAddress = _config["gRPCHostAddress"];
  readonly ILogger<MyBizService> _logger = _loggerFactory.CreateLogger<MyBizService>();

  public async Task<HelloReply> HelloAsync(HelloRequest req)
  {
    //## 全手動叫用 Grpc Cllient。
    using var channel = GrpcChannel.ForAddress(gRPCHostAddress);
    var invoker = channel.Intercept(new GrpcLoggerInterceptor(_loggerFactory));
    var client = new Greeter.GreeterClient(invoker);

    var headers = new Metadata();
    //headers.Add("Authorization", $"Bearer {AccessToken}");

    var reply = await client.SayHelloAsync(req, headers);

    _logger.LogInformation("MyBizService.HelloAsync SUCESS.");
    await channel.ShutdownAsync(); // 手動清除資源。其實 Dispose 時就會自動清除。
    return reply;
  }

  public async Task<(SampleReply? reply, string errMsg)> GetFullNameAsync(SampleRequest req)
  {
    if (req.FirstName == "logical" && req.LastName == "error")
      return (null, "測試發生邏輯錯誤。");

    // 透過 DI 叫用 Grpc Cllient。
    var reply = await _sampleClient.GetFullNameAsync(req);

    _logger.LogInformation("MyBizService.GetFullNameAsync SUCESS.");
    return (reply, Constant.Success);
  }

  public async Task<ProductSaveReply> SaveProductAsync()
  {
    // 透過 DI 叫用 Grpc Cllient。

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

    // 透過 DI 叫用 Grpc Cllient。
    var reply = await _prodClient.SaveProductAsync(dataModel);
    return reply;
  }

  public async Task<ProductListReply> GetProductListAsync()
  {
    // 透過 DI 叫用 Grpc Cllient。
    var reply = await _prodClient.GetProductsAsync(new Empty(),
                      deadline: DateTime.UtcNow.AddSeconds(5)); // 測試 deadline, 主機端要跑３秒。
    return reply;
  }

}

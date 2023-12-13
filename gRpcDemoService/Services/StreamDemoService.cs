using Grpc.Core;
using GrpcDemoService;

namespace GrpcDemoService.Services;

public class StreamDemoService : StreamDemo.StreamDemoBase
{
  readonly ILogger<StreamDemoService> _logger;
  Random _random;

  public StreamDemoService(ILogger<StreamDemoService> logger)
  {
    _logger = logger;
    _random = new Random();
  }

  public override async Task ServerStreamDemo(Test req, IServerStreamWriter<Test> replyStream, ServerCallContext ctx)
  {
    for (int i = 1; i <= 20; i++)
    {
      string message = $"Message {i}";
      await replyStream.WriteAsync(new Test { Message = message });
      _logger.LogInformation($"已回應：{message}");

      var delayTime = _random.Next(1, 2000);
      await Task.Delay(delayTime);
    }
  }

  public override async Task<Test> ClientStreamDemo(IAsyncStreamReader<Test> reqStream, ServerCallContext ctx)
  {
    int receiveCount = 0;
    while(await reqStream.MoveNext()) 
    {
      var msg = reqStream.Current;
      receiveCount++;
      _logger.LogInformation($"已收到：{msg.Message}");
    }

    _logger.LogInformation($"Client Streaming Completed.");
    return new Test { Message = $"茲收到訊息{receiveCount}條。" };
  }
}

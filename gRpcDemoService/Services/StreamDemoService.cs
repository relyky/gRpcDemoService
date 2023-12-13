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
}

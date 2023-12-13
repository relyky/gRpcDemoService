using Grpc.Core;
using GrpcDemoService;

namespace GrpcDemoService.Services;

public class StreamDemoService(ILogger<StreamDemoService> _logger)
  : StreamDemo.StreamDemoBase
{
  Random _random = new Random();

  public override async Task ServerStreamDemo(Test req, IServerStreamWriter<Test> replyStream, ServerCallContext ctx)
  {
    for (int i = 1; i <= 20; i++)
    {
      // 模擬處理時間
      var delayTime = _random.Next(1, 2000);
      await Task.Delay(delayTime);

      string message = $"Message {i}";
      await replyStream.WriteAsync(new Test { Message = message });
      _logger.LogInformation($"已回應：{message}");
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

  public override async Task BidirectionStreamDemo(IAsyncStreamReader<Test> reqStream, IServerStreamWriter<Test> replyStream, ServerCallContext ctx)
  {
    var taskList = new List<Task>();

    while(await reqStream.MoveNext())
    {
      var reqMsg = reqStream.Current;
      _logger.LogInformation($"已收到：{reqMsg.Message}");

      //## 再開執行緒平行處理上傳訊息
      var msgTask = Task.Run(async () =>
      {
        // 模擬處理時間
        var delayTime = _random.Next(1, 2000);
        await Task.Delay(delayTime);

        var replyMsg = new Test { Message = $"回應訊息 → {reqMsg.Message}" };
        await replyStream.WriteAsync(replyMsg);
        _logger.LogInformation($"已回應：{replyMsg.Message}");
      });

      taskList.Add(msgTask);
    }

    await Task.WhenAll(taskList);
    _logger.LogInformation("Bidirectional Streaming Completed.");
  }
}

using Grpc.Core;
using GrpcDemoService;

namespace GrpcDemoService.Services;

public class SampleService : Sample.SampleBase
{
  public override Task<SampleReply> GetFullName(SampleRequest req, ServerCallContext ctx)
  {
    try
    {
      if (req.FirstName == "throw" && req.LastName == "exception")
        throw new ApplicationException("測試 gRPC 出現例外！");

      var fullName = $"{req.FirstName} {req.LastName}";
      var result = new SampleReply() { FullName = fullName };
      return Task.FromResult(result);
    }
    catch(Exception ex)
    {
      /// 把 Exception 轉換成 RpcException 以識別錯誤訊息。
      /// 否則還是會自動轉成 RpcException(Unkonw) 這樣無法識別錯誤訊息。 => Status(StatusCode="Unknown", Detail="Exception was thrown by handler.")
      throw new RpcException(new Status(StatusCode.Aborted, ex.Message, ex));
    }
  }
}

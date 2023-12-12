using Grpc.Core;
using GrpcDemoService;

namespace GrpcDemoService.Services;

public class SampleService : Sample.SampleBase
{
  public override Task<SampleReply> GetFullName(SampleRequest req, ServerCallContext ctx)
  {
    var fullName = $"{req.FirstName} {req.LastName}";
    var result = new SampleReply() {  FullName = fullName };
    return Task.FromResult(result);
  }
}

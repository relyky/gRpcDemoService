using Grpc.Core;
using GrpcDemoService;

namespace GrpcDemoService.Services;

public class ProductService : Product.ProductBase
{
  readonly ILogger<ProductService> _logger;
  public ProductService(ILogger<ProductService> logger)
  {
    _logger = logger;
  }

  public override Task<ProductSaveReply> SaveProduct(ProductModel req, ServerCallContext ctx)
  {
    _logger.LogInformation($"{req.ProductName} | {req.ProductCode} | {req.Price}");

    var reply = new ProductSaveReply
    {
      StatusCode = 200,
      IsSuccessful = true,
    };

    return Task.FromResult(reply);
  }
}

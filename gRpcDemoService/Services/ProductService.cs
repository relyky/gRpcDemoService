using Google.Protobuf.WellKnownTypes;
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

  public override Task<ProductListReply> GetProducts(Empty req, ServerCallContext ctx)
  {
    var product1 = new ProductModel { ProductName = "Product 001", ProductCode = "P01", Price = 100 };
    var product2 = new ProductModel { ProductName = "Product 002", ProductCode = "P02", Price = 200 };
    var product3 = new ProductModel { ProductName = "Product 003", ProductCode = "P03", Price = 300 };

    var reply = new ProductListReply();
    reply.Products.Add(product1); 
    reply.Products.Add(product2);
    reply.Products.Add(product3);

    return Task.FromResult(reply);
  }
}

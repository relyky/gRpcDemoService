using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcDemoService;
using Microsoft.VisualBasic;
using static Google.Protobuf.WellKnownTypes.Field.Types;
using System.Reflection.Metadata;

namespace GrpcDemoService.Services;

public class ProductService(ILogger<ProductService> _logger)
  : Product.ProductBase
{
  public override Task<ProductSaveReply> SaveProduct(ProductModel req, ServerCallContext ctx)
  {
    try
    {
      _logger.LogInformation($"{req.ProductName} | {req.ProductCode} | {req.Price} | {req.StockDate}");

      var reply = new ProductSaveReply
      {
        StatusCode = 200,
        IsSuccessful = true,
      };

      return Task.FromResult(reply);
    }
    catch (Exception ex)
    {
      /// 把 Exception 轉換成 RpcException 以識別錯誤訊息。
      /// 否則還是會自動轉成 RpcException(Unkonw) 這樣無法識別錯誤訊息。 => Status(StatusCode="Unknown", Detail="Exception was thrown by handler.")
      throw new RpcException(new Status(StatusCode.Aborted, ex.Message, ex));
    }
  }

  public override async Task<ProductListReply> GetProducts(Empty req, ServerCallContext ctx)
  {
    try
    {
      await Task.Delay(5000); // 測試 deadline

      //※ gRCP 的日期格式必需是 UTC。
      //var stockDate = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2023, 12, 13), DateTimeKind.Utc));
      //var stockDate = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow);
      var stockDate = Timestamp.FromDateTime(new DateTime(2023, 12, 13).ToUniversalTime());

      var product1 = new ProductModel { ProductName = "Product 001", ProductCode = "P01", Price = 100, StockDate = stockDate };
      var product2 = new ProductModel { ProductName = "Product 002", ProductCode = "P02", Price = 200, StockDate = stockDate };
      var product3 = new ProductModel { ProductName = "Product 003", ProductCode = "P03", Price = 300, StockDate = stockDate };

      var reply = new ProductListReply();
      reply.Products.Add(product1);
      reply.Products.Add(product2);
      reply.Products.Add(product3);

      return reply;
    }
    catch (Exception ex)
    {
      /// 把 Exception 轉換成 RpcException 以識別錯誤訊息。
      /// 否則還是會自動轉成 RpcException(Unkonw) 這樣無法識別錯誤訊息。 => Status(StatusCode="Unknown", Detail="Exception was thrown by handler.")
      throw new RpcException(new Status(StatusCode.Aborted, ex.Message, ex));
    }
  }
}

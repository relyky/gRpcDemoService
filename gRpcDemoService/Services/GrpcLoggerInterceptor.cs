using Grpc.Core.Interceptors;
using Grpc.Core;
using System.Text;

namespace GrpcDemoService.Services;

/// <summary>
/// 參考：[.NET 上的 gRPC 攔截器](https://learn.microsoft.com/zh-tw/aspnet/core/grpc/interceptors?view=aspnetcore-6.0)
/// </summary>
public class GrpcLoggerInterceptor : Interceptor
{
  readonly ILogger<GrpcLoggerInterceptor> _logger;

  public GrpcLoggerInterceptor(ILogger<GrpcLoggerInterceptor> logger)
  {
    _logger = logger; // ---- 不知寫到那去了？
  }

  /// <summary>
  /// 伺服器攔截器 - Unary: 攔截一元 RPC。
  /// </summary>
  public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
      TRequest request,
      ServerCallContext context,
      UnaryServerMethod<TRequest, TResponse> continuation)
  {
    try
    {
      _logger.LogDebug($"Unary Starting: {MethodType.Unary} / {context.Method} ");
      var reply = await continuation(request, context);
      _logger.LogDebug($"Unary Done: {MethodType.Unary} / {context.Method} ");
      Console.WriteLine($"Unary Done: {MethodType.Unary} / {context.Method} ");
      return reply;
    }
    catch (Exception ex)
    {
      _logger.LogError($"Unary Exception: {MethodType.Unary} / {context.Method} → {ex.Message} ");
      throw;
    }
  }

  /// <summary>
  /// 伺服器攔截器 - Client Streaming: 攔截用戶端串流 RPC。
  /// </summary>
  public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
    IAsyncStreamReader<TRequest> requestStream,
    ServerCallContext context,
    ClientStreamingServerMethod<TRequest, TResponse> continuation)
  {
    try
    {
      _logger.LogDebug($"Client Streaming Starting: {MethodType.ClientStreaming} / {context.Method} ");
      var reply = await continuation(requestStream, context);
      _logger.LogDebug($"Client Streaming Done: {MethodType.ClientStreaming} / {context.Method} ");
      return reply;
    }
    catch (Exception ex)
    {
      _logger.LogError($"Client Streaming Exception: {MethodType.ClientStreaming} / {context.Method} → {ex.Message} ");
      throw;
    }
  }

  /// <summary>
  /// 伺服器攔截器 - Server Streaming: 攔截伺服器串流 RPC。
  /// </summary>
  public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
    TRequest request,
    IServerStreamWriter<TResponse> responseStream,
    ServerCallContext context,
    ServerStreamingServerMethod<TRequest, TResponse> continuation)
  {
    try
    {
      _logger.LogDebug($"Server Streaming Starting: {MethodType.ServerStreaming} / {context.Method} ");
      await continuation(request, responseStream, context);
      _logger.LogDebug($"Server Streaming Done: {MethodType.ServerStreaming} / {context.Method} ");
    }
    catch (Exception ex)
    {
      _logger.LogError($"Server Streaming Exception: {MethodType.ServerStreaming} / {context.Method} → {ex.Message} ");
      throw;
    }
  }

  /// <summary>
  /// 伺服器攔截器 - Duplex Streaming: 攔截雙向串流 RPC。
  /// </summary>
  public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
    IAsyncStreamReader<TRequest> requestStream,
    IServerStreamWriter<TResponse> responseStream,
    ServerCallContext context,
    DuplexStreamingServerMethod<TRequest, TResponse> continuation)
  {
    try
    {
      _logger.LogDebug($"Duplex Streaming Starting: {MethodType.DuplexStreaming} / {context.Method} ");
      await continuation(requestStream, responseStream, context);
      _logger.LogDebug($"Duplex Streaming Done: {MethodType.DuplexStreaming} / {context.Method} ");
    }
    catch (Exception ex)
    {
      _logger.LogError($"Duplex Streaming Exception: {MethodType.DuplexStreaming} / {context.Method} → {ex.Message} ");
      throw;
    }
  }

  //---------------------------------------------------------------------------

  /// <summary>
  /// 用戶端攔截器 - Unary: 攔截一元 RPC 的非同步叫用。
  /// </summary>
  public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
      TRequest request,
      ClientInterceptorContext<TRequest, TResponse> context,
      AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
  {
    try
    {
      _logger.LogDebug($"Unary Starting: {context.Method.Type} / {context.Method.Name} ");
      var reply = continuation(request, context);
      _logger.LogInformation($"Unary Done: {context.Method.Type} / {context.Method.Name} ");
      return reply;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Unary Exception: {context.Method.Type} / {context.Method.Name} → {ex.Message} ");
      throw;
    }
  }

  /// <summary>
  /// 用戶端攔截器 - Blocking Unary: 攔截一元 RPC 的封鎖叫用。
  /// </summary>
  public override TResponse BlockingUnaryCall<TRequest, TResponse>(
    TRequest request,
    ClientInterceptorContext<TRequest, TResponse> context,
    BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
  {
    try
    {
      _logger.LogDebug($"Blocking Unary Starting: {context.Method.Type} / {context.Method.Name} ");
      var reply = continuation(request, context);
      _logger.LogInformation($"Blocking Unary Done: {context.Method.Type} / {context.Method.Name} ");
      return reply;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Blocking Unary Exception: {context.Method.Type} / {context.Method.Name} → {ex.Message} ");
      throw;
    }
  }

  /// <summary>
  /// 用戶端攔截器 - Client Streaming: 攔截用戶端串流 RPC 的非同步叫用。
  /// </summary>
  public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(
    ClientInterceptorContext<TRequest, TResponse> context,
    AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
  {
    try
    {
      _logger.LogDebug($"Client Streaming Starting: {context.Method.Type} / {context.Method.Name} ");
      var reply = continuation(context);
      _logger.LogInformation($"Client Streaming Done: {context.Method.Type} / {context.Method.Name} ");
      return reply;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Client Streaming Exception: {context.Method.Type} / {context.Method.Name} → {ex.Message} ");
      throw;
    }
  }

  /// <summary>
  /// 用戶端攔截器 - Server Streaming: 攔截伺服器串流 RPC 的非同步叫用。
  /// </summary>
  public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(
    TRequest request,
    ClientInterceptorContext<TRequest, TResponse> context,
    AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
  {
    try
    {
      _logger.LogDebug($"Server Streaming Starting: {context.Method.Type} / {context.Method.Name} ");
      var reply = continuation(request, context);
      _logger.LogInformation($"Server Streaming Done: {context.Method.Type} / {context.Method.Name} ");
      return reply;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Server Streaming Exception: {context.Method.Type} / {context.Method.Name} → {ex.Message} ");
      throw;
    }
  }

  /// <summary>
  /// 用戶端攔截器 - Duplex Streaming: 攔截雙向串流 RPC 的非同步叫用。
  /// </summary>
  public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(
    ClientInterceptorContext<TRequest, TResponse> context,
    AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
  {
    try
    {
      _logger.LogDebug($"Duplex Streaming Starting: {context.Method.Type} / {context.Method.Name} ");
      var reply = continuation(context);
      _logger.LogInformation($"Duplex Streaming Done: {context.Method.Type} / {context.Method.Name} ");
      return reply;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Duplex Streaming Exception: {context.Method.Type} / {context.Method.Name} → {ex.Message} ");
      throw;
    }
  }
}

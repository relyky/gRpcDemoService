using Grpc.Core.Interceptors;
using Grpc.Core;
using System.Text;

namespace BlazorServerApp.Services;

/// <summary>
/// 參考：[.NET 上的 gRPC 攔截器](https://learn.microsoft.com/zh-tw/aspnet/core/grpc/interceptors?view=aspnetcore-6.0)
/// </summary>
public class GrpcLoggerInterceptor : Interceptor
{
  readonly ILogger<GrpcLoggerInterceptor> _logger;

  public GrpcLoggerInterceptor(ILoggerFactory loggerFactory)
  {
    _logger = loggerFactory.CreateLogger<GrpcLoggerInterceptor>();// ---- 不知寫到那去了？
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
      _logger.LogDebug($"伺服器攔截器 - Unary Starting: {MethodType.Unary} / {context.Method} ");
      var reply = await continuation(request, context);
      _logger.LogInformation($"伺服器攔截器 - Unary Done: {MethodType.Unary} / {context.Method} ");
      return reply;
    }
    catch (Exception ex)
    {
      _logger.LogError($"伺服器攔截器 - Unary Exception: {MethodType.Unary} / {context.Method} → {ex.Message} ");
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
      _logger.LogDebug($"伺服器攔截器 - Client Streaming Starting: {MethodType.ClientStreaming} / {context.Method} ");
      var reply = await continuation(requestStream, context);
      _logger.LogInformation($"伺服器攔截器 - Client Streaming Done: {MethodType.ClientStreaming} / {context.Method} ");
      return reply;
    }
    catch (Exception ex)
    {
      _logger.LogError($"伺服器攔截器 - Client Streaming Exception: {MethodType.ClientStreaming} / {context.Method} → {ex.Message} ");
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
      _logger.LogDebug($"伺服器攔截器 - Server Streaming Starting: {MethodType.ServerStreaming} / {context.Method} ");
      await continuation(request, responseStream, context);
      _logger.LogInformation($"伺服器攔截器 - Server Streaming Done: {MethodType.ServerStreaming} / {context.Method} ");
    }
    catch (Exception ex)
    {
      _logger.LogError($"伺服器攔截器 - Server Streaming Exception: {MethodType.ServerStreaming} / {context.Method} → {ex.Message} ");
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
      _logger.LogDebug($"伺服器攔截器 - Duplex Streaming Starting: {MethodType.DuplexStreaming} / {context.Method} ");
      await continuation(requestStream, responseStream, context);
      _logger.LogInformation($"伺服器攔截器 - Duplex Streaming Done: {MethodType.DuplexStreaming} / {context.Method} ");
    }
    catch (Exception ex)
    {
      _logger.LogError($"伺服器攔截器 - Duplex Streaming Exception: {MethodType.DuplexStreaming} / {context.Method} → {ex.Message} ");
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
      _logger.LogDebug($"用戶端攔截器 - Unary Starting: {context.Method.Type} / {context.Method.Name} ");
      var reply = continuation(request, context);
      _logger.LogInformation($"用戶端攔截器 - Unary Done: {context.Method.Type} / {context.Method.Name} ");
      return reply;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"用戶端攔截器 - Unary Exception: {context.Method.Type} / {context.Method.Name} → {ex.Message} ");
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
      _logger.LogDebug($"用戶端攔截器 - Blocking Unary Starting: {context.Method.Type} / {context.Method.Name} ");
      var reply = continuation(request, context);
      _logger.LogInformation($"用戶端攔截器 - Blocking Unary Done: {context.Method.Type} / {context.Method.Name} ");
      return reply;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"用戶端攔截器 - Blocking Unary Exception: {context.Method.Type} / {context.Method.Name} → {ex.Message} ");
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
      _logger.LogDebug($"用戶端攔截器 - Client Streaming Starting: {context.Method.Type} / {context.Method.Name} ");
      var reply = continuation(context);
      _logger.LogInformation($"用戶端攔截器 - Client Streaming Done: {context.Method.Type} / {context.Method.Name} ");
      return reply;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"用戶端攔截器 - Client Streaming Exception: {context.Method.Type} / {context.Method.Name} → {ex.Message} ");
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
      _logger.LogDebug($"用戶端攔截器 - Server Streaming Starting: {context.Method.Type} / {context.Method.Name} ");
      var reply = continuation(request, context);
      _logger.LogInformation($"用戶端攔截器 - Server Streaming Done: {context.Method.Type} / {context.Method.Name} ");
      return reply;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"用戶端攔截器 - Server Streaming Exception: {context.Method.Type} / {context.Method.Name} → {ex.Message} ");
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
      _logger.LogDebug($"用戶端攔截器 - Duplex Streaming Starting: {context.Method.Type} / {context.Method.Name} ");
      var reply = continuation(context);
      _logger.LogInformation($"用戶端攔截器 - Duplex Streaming Done: {context.Method.Type} / {context.Method.Name} ");
      return reply;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"用戶端攔截器 - Duplex Streaming Exception: {context.Method.Type} / {context.Method.Name} → {ex.Message} ");
      throw;
    }
  }
}

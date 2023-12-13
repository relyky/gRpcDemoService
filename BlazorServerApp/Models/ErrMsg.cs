//namespace BlazorServerApp.Models;

//internal record ErrMsg
//{
//  public string Message = string.Empty;
//  public LogLevel Level = LogLevel.Information;
//  public Exception? Exception = null;

//  public bool IsSuccess => Level == LogLevel.Information && Message == "SUCCESS";

//  // 常用
//  public static readonly ErrMsg Success = new ErrMsg { Level = LogLevel.Information, Message = "SUCCESS" };
//  public static ErrMsg Error(string message) => new ErrMsg { Level = LogLevel.Error, Message = message };
//  public static ErrMsg Critical(Exception ex, string message) => new ErrMsg { Level = LogLevel.Critical, Message = message, Exception = ex };
//}

//internal static class MyLoggerClassExtensions
//{
//  public static void Log(this ILogger _logger, ErrMsg msg) => _logger.Log(msg.Level, msg.Exception, msg.Message);
//}

internal static class Constant
{
  public static string Success => "SUCCESS";
}
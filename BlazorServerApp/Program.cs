using BlazorServerApp.Services;
using Grpc.Core;
using GrpcDemoService;
using System.Reflection.Metadata.Ecma335;

var builder = WebApplication.CreateBuilder(args);
var _config = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

//## 註冊：gRPC 攔截器
builder.Services.AddSingleton<GrpcLoggerInterceptor>();
builder.Services.AddScoped<ITokenProvider, AccountService>();

Func<AuthInterceptorContext, Metadata, IServiceProvider, Task> grpcAuthInterceptor = async (context, metadata, provider) =>
{
  var tokenProvider = provider.GetRequiredService<ITokenProvider>();
  var accessToken = await tokenProvider.GetTokenAsync(context.CancellationToken);
  metadata.Add("Authorization", $"Bearer {accessToken}");
};

//## 註冊：gRPC Client Servic
builder.Services.AddGrpcClient<Sample.SampleClient>(options =>
{
  options.Address = new Uri(_config["gRPCHostAddress"]);

  //## 進階組態：保持運作 Ping - 應用於大量且需即時回應的情境，一般不需要開啟此組態。
  // ref → [Keep alive pings/保持運作 Ping](https://learn.microsoft.com/zh-tw/aspnet/core/grpc/performance?view=aspnetcore-6.0#keep-alive-pings)
  options.ChannelOptionsActions.Add(channelOptions =>
    channelOptions.HttpHandler = new SocketsHttpHandler
    {
      PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
      KeepAlivePingDelay = TimeSpan.FromSeconds(60),
      KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
      EnableMultipleHttp2Connections = true
    });

}).AddCallCredentials(grpcAuthInterceptor)
  .AddInterceptor<GrpcLoggerInterceptor>();

builder.Services.AddGrpcClient<Product.ProductClient>(options =>
{
  options.Address = new Uri(_config["gRPCHostAddress"]);
}).AddCallCredentials(grpcAuthInterceptor)
  .AddInterceptor<GrpcLoggerInterceptor>();

builder.Services.AddGrpcClient<StreamDemo.StreamDemoClient>(options =>
{
  options.Address = new Uri(_config["gRPCHostAddress"]);
}).AddCallCredentials(grpcAuthInterceptor)
  .AddInterceptor<GrpcLoggerInterceptor>();

//## 註冊：客製服務
builder.Services.AddSingleton<AccountService>();
builder.Services.AddScoped<MyBizService>();
builder.Services.AddScoped<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

Console.WriteLine("BlazorServerApp is runing.");
app.Run();

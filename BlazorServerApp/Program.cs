using BlazorServerApp.Services;
using Grpc.Core;
using GrpcDemoService;

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
builder.Services.AddGrpcClient<Greeter.GreeterClient>(options =>
{
  options.Address = new Uri(_config["gRPCHostAddress"]);
}).AddCallCredentials(grpcAuthInterceptor)
  .AddInterceptor<GrpcLoggerInterceptor>();

builder.Services.AddGrpcClient<Sample.SampleClient>(options =>
{
  options.Address = new Uri(_config["gRPCHostAddress"]);
}).AddCallCredentials(grpcAuthInterceptor)
  .AddInterceptor<GrpcLoggerInterceptor>();

builder.Services.AddGrpcClient<Product.ProductClient>(options =>
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

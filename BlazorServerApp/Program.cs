using BlazorServerApp.Services;
using Grpc.Net.Client;
using GrpcDemoService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

//## ���U�GgRPC �d�I��
builder.Services.AddSingleton<GrpcLoggerInterceptor>();

//## ���U�GgRPC Client Service
builder.Services.AddGrpcClient<Product.ProductClient>(options =>
{
  options.Address = new Uri(@"https://localhost:7176");
}).AddInterceptor<GrpcLoggerInterceptor>();

//## ���U�G�Ȼs�A��
builder.Services.AddSingleton<AccountService>();
builder.Services.AddScoped<MyGrpcClient>();
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

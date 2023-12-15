using GrpcDemoService;
using GrpcDemoService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

//## JWT JwtAuthentication
builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
  options.RequireHttpsMetadata = false;
  options.SaveToken = true;
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtAuthenticationManager.JWT_TOKEN_KEY)),
    TokenDecryptionKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtAuthenticationManager.JWE_SECRET_KEY)), // JWE
    ValidateIssuer = false,     // true; 正式版應該要加入驗證，然實際意義不大。
    ValidateAudience = false,   // true; 正式版應該要加入驗證，然實際意義不大。
    ClockSkew = TimeSpan.Zero,  // 時鐘偏差，預設為5分鐘。當 expires 時間過了仍有小段時間有效。
  };
});

// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddGrpc();

var app = builder.Build();

app.UseRouting();

// for Authentication
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<SampleService>();
app.MapGrpcService<ProductService>();
app.MapGrpcService<StreamDemoService>();
app.MapGrpcService<AuthenticationService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

Console.WriteLine("GrpcDemoService is runing.");
app.Run();

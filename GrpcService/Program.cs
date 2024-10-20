using EpcDataApp.GrpcService;
using EpcDataApp.GrpcService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("postgresql");

builder.Services.AddGrpc();
builder.Services.AddDbContext<TestDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

app.MapGrpcService<EpcDataService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

app.Run();

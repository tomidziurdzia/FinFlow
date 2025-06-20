using FinFlow.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization();

var app = builder.Build();

app.Run();

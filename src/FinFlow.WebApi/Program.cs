using FinFlow.Infrastructure;
using FinFlow.WebApi;
using FinFlow.WebApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructureServices(builder.Configuration)
    .AddWebApiServices(builder.Configuration);


var app = builder.Build();

app.UseApiServices();
app.RegisterUserEndpoints()
    .RegisterCategoryEndpoints();

app.Run();
using FinFlow.Application.Contracts.Categories;
using FinFlow.Application.Contracts.Users;
using FinFlow.Application.Implementation.Categories;
using FinFlow.Application.Implementation.Users;
using FinFlow.Infrastructure.Services.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace FinFlow.WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        var frontendUrl = configuration["FrontendUrl"] ?? "http://localhost:3000";
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy
                    .WithOrigins(frontendUrl)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        
        services.AddHttpContextAccessor();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var clerkDomain = configuration["Clerk:Domain"]!;
                options.Authority = $"https://{clerkDomain}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://{clerkDomain}",
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.MaxValue
                };
            });
        
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICategoryService, CategoryService>();

        services.AddAuthorization();
        services.AddEndpointsApiExplorer();

        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        app.UseCors("CorsPolicy");
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
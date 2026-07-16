// VYUH Engine - Gateway API
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VYUH.Gateway.Application;
using VYUH.Gateway.Infrastructure.Persistence;
using VYUH.Gateway.Infrastructure.Services;

using Scalar.AspNetCore;
using VYUH.Gateway.Api.Filters;
using VYUH.Gateway.Api.Hubs;
using VYUH.Gateway.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register PostgreSQL DbContext
builder.Services.AddDbContext<GatewayDbContext>(options =>
{
    var conn = builder.Configuration["ConnectionStrings:PostgreSQL"] ?? "Host=localhost;Database=vyuh_db;Username=postgres;Password=Password123";
    options.UseNpgsql(conn);
});

// Register Application Repositories
builder.Services.AddScoped<IOrderExecutionLogRepository, OrderExecutionLogRepository>();
builder.Services.AddScoped<IPositionReconciliationMismatchRepository, PositionReconciliationMismatchRepository>();
builder.Services.AddScoped<IUserAuditLogRepository, UserAuditLogRepository>();

// Register Filters
builder.Services.AddScoped<HmacSignatureFilter>();
builder.Services.AddScoped<AuditLogFilter>();

// Register Broker Client
builder.Services.AddScoped<IBrokerClient, MockBrokerClient>();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = VYUH.Gateway.Api.Helpers.JwtTokenHelper.Issuer,
        ValidAudience = VYUH.Gateway.Api.Helpers.JwtTokenHelper.Audience,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(VYUH.Gateway.Api.Helpers.JwtTokenHelper.SecretKey))
    };
});

builder.Services.AddAuthorization();

// Register SignalR
builder.Services.AddSignalR();
builder.Services.AddScoped<IRiskAlertService, RiskAlertService>();
builder.Services.AddHostedService<PortfolioMetricsBroadcaster>();

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IBrokerClient).Assembly));

// Register Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<PortfolioStreamHub>("/hubs/portfolio-stream");

app.MapHealthChecks("/health/liveness");
app.MapHealthChecks("/health/readiness");

app.MapGet("/", () => "VYUH Engine - Gateway Service is Active and Healthy.");

app.Run();

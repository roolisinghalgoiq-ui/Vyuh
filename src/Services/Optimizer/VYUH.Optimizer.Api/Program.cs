// VYUH Engine - Optimizer API
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using VYUH.Optimizer.Application;
using VYUH.Optimizer.Application.Interfaces;
using VYUH.Optimizer.Application.Services;
using VYUH.Optimizer.Infrastructure.Persistence;
using VYUH.Optimizer.Infrastructure.Services;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register PostgreSQL DbContext
builder.Services.AddDbContext<OptimizerDbContext>(options =>
{
    var conn = builder.Configuration["ConnectionStrings:PostgreSQL"] ?? "Host=localhost;Database=vyuh_db;Username=postgres;Password=Password123";
    options.UseNpgsql(conn);
});

// Register Redis Multiplexer as a Singleton
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => 
{
    var host = builder.Configuration["Redis:Host"] ?? "localhost:6379";
    return ConnectionMultiplexer.Connect(host);
});

// Register Ingestion Service Client using HTTP Client Factory
builder.Services.AddHttpClient<IIngestionServiceClient, IngestionServiceClient>(client =>
{
    var address = builder.Configuration["IngestionService:Address"] ?? "http://localhost:5001";
    client.BaseAddress = new Uri(address);
});

// Register Application Repositories & Services
builder.Services.AddScoped<IStockScoreArchiveRepository, StockScoreArchiveRepository>();
builder.Services.AddScoped<IFilterThresholdRepository, FilterThresholdRepository>();
builder.Services.AddScoped<IStrikeSelectionConfigRepository, StrikeSelectionConfigRepository>();
builder.Services.AddScoped<IAiParameterProposalRepository, AiParameterProposalRepository>();
builder.Services.AddScoped<IRedisSortedSetService, RedisSortedSetService>();
builder.Services.AddSingleton<BootstrapSimulationEngine>();

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IIngestionServiceClient).Assembly));

// Register Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();

app.MapHealthChecks("/health/liveness");
app.MapHealthChecks("/health/readiness");

app.MapGet("/", () => "VYUH Engine - Optimizer Service is Active and Healthy.");

app.Run();

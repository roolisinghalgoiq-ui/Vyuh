// VYUH Engine - Ingestion API
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using VYUH.Ingestion.Application;
using VYUH.Ingestion.Application.Interfaces;
using VYUH.Ingestion.Infrastructure;
using VYUH.Ingestion.Infrastructure.Persistence;
using VYUH.Ingestion.Infrastructure.Services;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register PostgreSQL DbContext
builder.Services.AddDbContext<IngestionDbContext>(options =>
{
    var conn = builder.Configuration["ConnectionStrings:PostgreSQL"] ?? "Host=localhost;Database=vyuh_db;Username=postgres;Password=algo@123";
    options.UseNpgsql(conn);
});

// Register Redis Multiplexer as a Singleton
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => 
{
    var host = builder.Configuration["Redis:Host"] ?? "localhost:6379";
    return ConnectionMultiplexer.Connect(host);
});

// Register Application Repositories and Services
builder.Services.AddScoped<IOptionChainRepository, RedisOptionChainRepository>();
builder.Services.AddScoped<IOptionLiquidityLogRepository, OptionLiquidityLogRepository>();
builder.Services.AddScoped<ISuchakClient, SuchakClient>();

// Register TalkOptions Client via HttpClient factory
builder.Services.AddHttpClient<ITalkOptionsClient, TalkOptionsClient>(client =>
{
    var address = builder.Configuration["TalkOptions:Address"] ?? "http://localhost:50090";
    client.BaseAddress = new Uri(address);
});

// Register Ganesh Client with configured address
builder.Services.AddSingleton<IGaneshClient>(sp =>
{
    var address = builder.Configuration["Ganesh:Address"] ?? "http://localhost:50051";
    return new GaneshClient(address);
});

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IOptionChainRepository).Assembly));

// Register Background Worker
builder.Services.AddHostedService<KafkaOptionChainConsumer>();

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

app.MapGet("/", () => "VYUH Engine - Ingestion Service is Active and Healthy.");

app.Run();

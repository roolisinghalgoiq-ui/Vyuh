// VYUH Engine - Risk API
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VYUH.Risk.Application;
using VYUH.Risk.Application.Interfaces;
using VYUH.Risk.Infrastructure.Persistence;
using VYUH.Risk.Infrastructure.Services;
using Scalar.AspNetCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register PostgreSQL DbContext
builder.Services.AddDbContext<RiskDbContext>(options =>
{
    var conn = builder.Configuration["ConnectionStrings:PostgreSQL"] ?? "Host=localhost;Database=vyuh_db;Username=postgres;Password=Password123";
    options.UseNpgsql(conn);
});

// Register Application Repositories
builder.Services.AddScoped<IMarginMultiplierRepository, MarginMultiplierRepository>();
builder.Services.AddScoped<IExitConfigRepository, ExitConfigRepository>();
builder.Services.AddScoped<IVarAuditLogRepository, VarAuditLogRepository>();
builder.Services.AddHttpClient<IIngestionServiceClient, IngestionServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:Ingestion"] ?? "http://localhost:5001");
});

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IMarginMultiplierRepository).Assembly));

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

app.MapGet("/", () => "VYUH Engine - Risk Service is Active and Healthy.");

app.Run();

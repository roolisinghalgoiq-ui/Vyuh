using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Gateway.Api.Hubs;

namespace VYUH.Gateway.Api.Services;

public class PortfolioMetricsBroadcaster : BackgroundService
{
    private readonly IHubContext<PortfolioStreamHub> _hubContext;
    private readonly Random _random = new();

    public PortfolioMetricsBroadcaster(IHubContext<PortfolioStreamHub> hubContext)
    {
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var update = new
            {
                portfolioTheta = 450000.00 + _random.Next(-5000, 5000),
                portfolioDelta = 12.0 + (_random.NextDouble() * 2.0 - 1.0),
                marginUsagePct = 58.0 + (_random.NextDouble() * 2.0 - 1.0),
                unrealizedPnL = 1420000.00 + _random.Next(-20000, 20000),
                lastEvaluationTime = DateTime.UtcNow
            };

            await _hubContext.Clients.Group(PortfolioStreamHub.PortfolioMetricsGroup)
                .SendAsync("ReceivePortfolioUpdate", update, stoppingToken);

            await Task.Delay(10000, stoppingToken);
        }
    }
}

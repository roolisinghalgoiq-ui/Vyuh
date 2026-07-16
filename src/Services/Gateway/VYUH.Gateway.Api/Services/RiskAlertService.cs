using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using VYUH.Gateway.Api.Hubs;
using VYUH.Gateway.Application;

namespace VYUH.Gateway.Api.Services;

public class RiskAlertService : IRiskAlertService
{
    private readonly IHubContext<PortfolioStreamHub> _hubContext;

    public RiskAlertService(IHubContext<PortfolioStreamHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task BroadcastBreachAsync(string breachType, string stockId, decimal currentMarginUsage, decimal limit, string severity)
    {
        var breach = new
        {
            breachType,
            stockId,
            currentMarginUsage,
            limit,
            severity
        };

        await _hubContext.Clients.Group(PortfolioStreamHub.BreachesGroup)
            .SendAsync("ReceiveRiskBreach", breach);
    }
}

using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace VYUH.Gateway.Api.Hubs;

public class PortfolioStreamHub : Hub
{
    public const string PortfolioMetricsGroup = "PortfolioMetrics";
    public const string BreachesGroup = "Breaches";

    public async Task SubscribeToPortfolioMetrics()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, PortfolioMetricsGroup);
    }

    public async Task SubscribeToBreaches()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, BreachesGroup);
    }

    public async Task UnsubscribeFromPortfolioMetrics()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, PortfolioMetricsGroup);
    }

    public async Task UnsubscribeFromBreaches()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, BreachesGroup);
    }
}

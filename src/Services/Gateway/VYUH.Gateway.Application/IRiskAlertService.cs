using System.Threading.Tasks;

namespace VYUH.Gateway.Application;

public interface IRiskAlertService
{
    Task BroadcastBreachAsync(string breachType, string stockId, decimal currentMarginUsage, decimal limit, string severity);
}

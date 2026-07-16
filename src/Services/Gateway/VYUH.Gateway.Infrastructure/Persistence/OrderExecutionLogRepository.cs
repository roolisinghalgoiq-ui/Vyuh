using System.Threading.Tasks;
using VYUH.Gateway.Application;
using VYUH.Gateway.Domain;

namespace VYUH.Gateway.Infrastructure.Persistence;

public class OrderExecutionLogRepository : IOrderExecutionLogRepository
{
    private readonly GatewayDbContext _dbContext;

    public OrderExecutionLogRepository(GatewayDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddLogAsync(OrderExecutionLog log)
    {
        _dbContext.OrderExecutionLogs.Add(log);
        await _dbContext.SaveChangesAsync();
    }
}

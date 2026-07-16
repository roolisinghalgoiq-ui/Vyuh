using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VYUH.Risk.Application;
using VYUH.Risk.Domain;

namespace VYUH.Risk.Infrastructure.Persistence;

public class ExitConfigRepository : IExitConfigRepository
{
    private readonly RiskDbContext _dbContext;

    public ExitConfigRepository(RiskDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ExitConfig?> GetExitConfigAsync(string stockId)
    {
        return await _dbContext.ExitConfigs.FirstOrDefaultAsync(c => c.StockId == stockId);
    }
}

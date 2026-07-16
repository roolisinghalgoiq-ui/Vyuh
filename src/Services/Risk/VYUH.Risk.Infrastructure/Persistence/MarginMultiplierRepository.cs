using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VYUH.Risk.Application;
using VYUH.Risk.Domain;

namespace VYUH.Risk.Infrastructure.Persistence;

public class MarginMultiplierRepository : IMarginMultiplierRepository
{
    private readonly RiskDbContext _dbContext;

    public MarginMultiplierRepository(RiskDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MarginMultiplier?> GetMultiplierAsync(string stockId)
    {
        return await _dbContext.MarginMultipliers.FirstOrDefaultAsync(m => m.StockId == stockId);
    }
}

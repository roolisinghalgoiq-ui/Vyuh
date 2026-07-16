using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Interfaces;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Infrastructure.Persistence;

public class StrikeSelectionConfigRepository : IStrikeSelectionConfigRepository
{
    private readonly OptimizerDbContext _dbContext;

    public StrikeSelectionConfigRepository(OptimizerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StrikeSelectionConfig?> GetConfigAsync(string settingId)
    {
        return await _dbContext.StrikeSelectionConfigs.FirstOrDefaultAsync(c => c.SettingId == settingId);
    }

    public async Task UpdateConfigAsync(StrikeSelectionConfig config)
    {
        _dbContext.StrikeSelectionConfigs.Update(config);
        await _dbContext.SaveChangesAsync();
    }
}

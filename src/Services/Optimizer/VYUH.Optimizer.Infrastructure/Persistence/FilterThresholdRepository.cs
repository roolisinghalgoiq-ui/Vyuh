using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Interfaces;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Infrastructure.Persistence;

public class FilterThresholdRepository : IFilterThresholdRepository
{
    private readonly OptimizerDbContext _dbContext;

    public FilterThresholdRepository(OptimizerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FilteringThreshold?> GetThresholdAsync(string filterId)
    {
        return await _dbContext.FilteringThresholds.FirstOrDefaultAsync(t => t.FilterId == filterId);
    }

    public async Task UpdateThresholdAsync(FilteringThreshold threshold)
    {
        _dbContext.FilteringThresholds.Update(threshold);
        await _dbContext.SaveChangesAsync();
    }
}

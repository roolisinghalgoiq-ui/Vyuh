using System.Threading.Tasks;
using VYUH.Optimizer.Application;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Infrastructure.Persistence;

public class StockScoreArchiveRepository : IStockScoreArchiveRepository
{
    private readonly OptimizerDbContext _dbContext;

    public StockScoreArchiveRepository(OptimizerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveScoreAsync(StockScoreArchive score)
    {
        _dbContext.StockScores.Add(score);
        await _dbContext.SaveChangesAsync();
    }
}

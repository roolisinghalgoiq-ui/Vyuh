using System.Collections.Generic;
using System.Threading.Tasks;
using VYUH.Ingestion.Application;
using VYUH.Ingestion.Domain;
using VYUH.Ingestion.Infrastructure.Persistence;

namespace VYUH.Ingestion.Infrastructure;

public class OptionLiquidityLogRepository : IOptionLiquidityLogRepository
{
    private readonly IngestionDbContext _dbContext;

    public OptionLiquidityLogRepository(IngestionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveLogsAsync(List<OptionLiquidityLog> logs)
    {
        _dbContext.OptionLiquidityLogs.AddRange(logs);
        await _dbContext.SaveChangesAsync();
    }
}

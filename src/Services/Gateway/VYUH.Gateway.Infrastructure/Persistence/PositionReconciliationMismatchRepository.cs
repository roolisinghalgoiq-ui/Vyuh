using System.Threading.Tasks;
using VYUH.Gateway.Application;
using VYUH.Gateway.Domain;

namespace VYUH.Gateway.Infrastructure.Persistence;

public class PositionReconciliationMismatchRepository : IPositionReconciliationMismatchRepository
{
    private readonly GatewayDbContext _dbContext;

    public PositionReconciliationMismatchRepository(GatewayDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddMismatchAsync(PositionReconciliationMismatch mismatch)
    {
        _dbContext.PositionReconciliationMismatches.Add(mismatch);
        await _dbContext.SaveChangesAsync();
    }
}

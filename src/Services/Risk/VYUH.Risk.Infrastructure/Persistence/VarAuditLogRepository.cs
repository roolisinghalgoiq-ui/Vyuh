using System.Threading.Tasks;
using VYUH.Risk.Application;
using VYUH.Risk.Domain;

namespace VYUH.Risk.Infrastructure.Persistence;

public class VarAuditLogRepository : IVarAuditLogRepository
{
    private readonly RiskDbContext _dbContext;

    public VarAuditLogRepository(RiskDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddLogAsync(VarAuditLog log)
    {
        _dbContext.VarAuditLogs.Add(log);
        await _dbContext.SaveChangesAsync();
    }
}

using System.Threading.Tasks;
using VYUH.Gateway.Application;
using VYUH.Gateway.Domain;

namespace VYUH.Gateway.Infrastructure.Persistence;

public class UserAuditLogRepository : IUserAuditLogRepository
{
    private readonly GatewayDbContext _context;

    public UserAuditLogRepository(GatewayDbContext context)
    {
        _context = context;
    }

    public async Task SaveAuditLogAsync(UserAuditLog log)
    {
        await _context.UserAuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }
}

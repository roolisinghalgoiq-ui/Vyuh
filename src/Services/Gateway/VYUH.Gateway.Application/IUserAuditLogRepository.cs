using System.Threading.Tasks;
using VYUH.Gateway.Domain;

namespace VYUH.Gateway.Application;

public interface IUserAuditLogRepository
{
    Task SaveAuditLogAsync(UserAuditLog log);
}

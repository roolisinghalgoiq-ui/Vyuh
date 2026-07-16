using System.Threading.Tasks;
using VYUH.Risk.Domain;

namespace VYUH.Risk.Application;

public interface IVarAuditLogRepository
{
    Task AddLogAsync(VarAuditLog log);
}

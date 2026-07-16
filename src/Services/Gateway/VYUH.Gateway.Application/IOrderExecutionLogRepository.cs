using System.Threading.Tasks;
using VYUH.Gateway.Domain;

namespace VYUH.Gateway.Application;

public interface IOrderExecutionLogRepository
{
    Task AddLogAsync(OrderExecutionLog log);
}

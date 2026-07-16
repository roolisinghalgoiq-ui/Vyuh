using System.Threading.Tasks;
using VYUH.Risk.Domain;

namespace VYUH.Risk.Application;

public interface IExitConfigRepository
{
    Task<ExitConfig?> GetExitConfigAsync(string stockId);
}

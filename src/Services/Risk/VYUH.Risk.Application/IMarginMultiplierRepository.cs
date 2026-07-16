using System.Threading.Tasks;
using VYUH.Risk.Domain;

namespace VYUH.Risk.Application;

public interface IMarginMultiplierRepository
{
    Task<MarginMultiplier?> GetMultiplierAsync(string stockId);
}

using System.Threading.Tasks;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Application.Interfaces;

public interface IFilterThresholdRepository
{
    Task<FilteringThreshold?> GetThresholdAsync(string filterId);
    Task UpdateThresholdAsync(FilteringThreshold threshold);
}

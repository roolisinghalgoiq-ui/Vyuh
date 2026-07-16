using System.Threading.Tasks;
using VYUH.Optimizer.Domain;

namespace VYUH.Optimizer.Application.Interfaces;

public interface IStrikeSelectionConfigRepository
{
    Task<StrikeSelectionConfig?> GetConfigAsync(string settingId);
    Task UpdateConfigAsync(StrikeSelectionConfig config);
}

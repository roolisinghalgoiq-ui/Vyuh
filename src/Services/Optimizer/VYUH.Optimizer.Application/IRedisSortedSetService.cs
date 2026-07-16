using System.Threading.Tasks;

namespace VYUH.Optimizer.Application;

public interface IRedisSortedSetService
{
    Task AddToSortedSetAsync(string key, string member, double score);
}

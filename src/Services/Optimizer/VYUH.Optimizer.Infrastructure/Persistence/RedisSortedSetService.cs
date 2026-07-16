using StackExchange.Redis;
using System.Threading.Tasks;
using VYUH.Optimizer.Application;

namespace VYUH.Optimizer.Infrastructure.Persistence;

public class RedisSortedSetService : IRedisSortedSetService
{
    private readonly IDatabase _redisDb;

    public RedisSortedSetService(IConnectionMultiplexer redisConnection)
    {
        _redisDb = redisConnection.GetDatabase();
    }

    public async Task AddToSortedSetAsync(string key, string member, double score)
    {
        await _redisDb.SortedSetAddAsync(key, member, score);
    }
}

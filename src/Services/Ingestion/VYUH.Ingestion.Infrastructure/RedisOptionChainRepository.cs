using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;
using VYUH.Ingestion.Application;
using VYUH.Ingestion.Domain;

namespace VYUH.Ingestion.Infrastructure;

public class RedisOptionChainRepository : IOptionChainRepository
{
    private readonly IDatabase _database;

    public RedisOptionChainRepository(IConnectionMultiplexer redisConnection)
    {
        _database = redisConnection.GetDatabase();
    }

    public async Task SaveSnapshotAsync(OptionChainSnapshot snapshot)
    {
        var key = $"vyuh:market:chain:{snapshot.StockId}:{snapshot.ExpiryDate}";
        var json = JsonSerializer.Serialize(snapshot);
        await _database.StringSetAsync(key, json);
    }

    public async Task<OptionChainSnapshot?> GetSnapshotAsync(string stockId, string expiryDate)
    {
        var key = $"vyuh:market:chain:{stockId}:{expiryDate}";
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null;
        return JsonSerializer.Deserialize<OptionChainSnapshot>(json!);
    }
}

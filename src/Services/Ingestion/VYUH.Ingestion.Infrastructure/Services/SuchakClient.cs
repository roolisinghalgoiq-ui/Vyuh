using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;
using VYUH.Ingestion.Application.Interfaces;
using VYUH.Ingestion.Domain;

namespace VYUH.Ingestion.Infrastructure.Services;

public class SuchakClient : ISuchakClient
{
    private readonly IDatabase _database;

    public SuchakClient(IConnectionMultiplexer redisConnection)
    {
        _database = redisConnection.GetDatabase();
    }

    public async Task<TechnicalIndicatorSnapshot> GetTechnicalIndicatorsAsync(string stockId)
    {
        var suchakKey = $"suchak:indicators:{stockId}";
        var vyuhKey = $"vyuh:suchak:{stockId}";

        try
        {
            var rawValue = await _database.StringGetAsync(suchakKey);
            TechnicalIndicatorSnapshot? snapshot = null;

            if (!rawValue.IsNullOrEmpty)
            {
                snapshot = JsonSerializer.Deserialize<TechnicalIndicatorSnapshot>(rawValue!);
            }

            if (snapshot == null)
            {
                snapshot = CreateMockSnapshot(stockId);
            }

            var json = JsonSerializer.Serialize(snapshot);
            await _database.StringSetAsync(vyuhKey, json);

            return snapshot;
        }
        catch
        {
            return CreateMockSnapshot(stockId);
        }
    }

    private TechnicalIndicatorSnapshot CreateMockSnapshot(string stockId)
    {
        return new TechnicalIndicatorSnapshot
        {
            StockId = stockId,
            Rsi = 54.20,
            Ema20 = 2445.50,
            Ema50 = 2420.00,
            Ema200 = 2310.50,
            Sma20 = 2440.10,
            Sma50 = 2425.20,
            Vwap = 2448.00,
            Adx = 22.40,
            MacdLine = 5.20,
            SignalLine = 4.10,
            SupertrendDirection = "BUY",
            SupertrendValue = 2380.00,
            SupportLevels = new List<double> { 2400.00, 2350.00, 2300.00 },
            ResistanceLevels = new List<double> { 2480.00, 2520.00, 2600.00 },
            LastUpdated = DateTime.UtcNow
        };
    }
}

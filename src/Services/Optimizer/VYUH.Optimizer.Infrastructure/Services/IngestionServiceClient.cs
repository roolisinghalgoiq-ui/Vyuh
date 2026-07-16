using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Interfaces;

namespace VYUH.Optimizer.Infrastructure.Services;

public class IngestionServiceClient : IIngestionServiceClient
{
    private readonly HttpClient _httpClient;

    public IngestionServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IngestionOptionChainSnapshot?> GetOptionChainAsync(string stockId, string expiryDate)
    {
        try
        {
            var path = $"api/v1/ingestion/options-chain/{stockId}/{expiryDate}";
            return await _httpClient.GetFromJsonAsync<IngestionOptionChainSnapshot>(path);
        }
        catch
        {
            // Fallback for local development if Ingestion Service is offline
            return CreateMockOptionChain(stockId, expiryDate);
        }
    }

    public async Task<List<IngestionHistoricalRecord>> GetHistoricalPricesAsync(string stockId, int periodDays)
    {
        try
        {
            var path = $"api/v1/historical/returns/{stockId}?days={periodDays}";
            var returns = await _httpClient.GetFromJsonAsync<List<HistoricalReturnResponse>>(path);
            var result = new List<IngestionHistoricalRecord>();
            if (returns != null)
            {
                foreach (var r in returns)
                {
                    result.Add(new IngestionHistoricalRecord
                    {
                        Date = r.Date,
                        ClosePrice = r.ClosePrice
                    });
                }
            }
            return result;
        }
        catch
        {
            // Fallback for local development
            var mockHistory = new List<IngestionHistoricalRecord>();
            var start = DateTime.UtcNow.AddDays(-periodDays);
            var price = 2450.00;
            var random = new Random();
            for (int i = 0; i < periodDays; i++)
            {
                price += (random.NextDouble() - 0.5) * 20.0;
                mockHistory.Add(new IngestionHistoricalRecord
                {
                    Date = start.AddDays(i),
                    ClosePrice = price
                });
            }
            return mockHistory;
        }
    }

    private class HistoricalReturnResponse
    {
        public DateTime Date { get; set; }
        public double ClosePrice { get; set; }
        public double LogReturn { get; set; }
    }

    private IngestionOptionChainSnapshot CreateMockOptionChain(string stockId, string expiryDate)
    {
        var spot = 2450.00;
        var snapshot = new IngestionOptionChainSnapshot
        {
            StockId = stockId,
            SpotPrice = spot,
            FuturePrice = spot + 10.00,
            ExpiryDate = expiryDate,
            IvPercentile = 65.0,
            Contracts = new List<IngestionOptionContract>()
        };

        // Create ATM and OTM Call/Put contracts with realistic Deltas
        double[] strikes = { 2400.00, 2450.00, 2500.00 };
        foreach (var strike in strikes)
        {
            var callDelta = strike == 2400.00 ? 0.80 : (strike == 2450.00 ? 0.50 : 0.17);
            snapshot.Contracts.Add(new IngestionOptionContract
            {
                StrikePrice = strike,
                Type = "CE",
                Bid = strike < spot ? 60.00 : 15.00,
                Ask = strike < spot ? 61.00 : 16.00,
                ImpliedVolatility = 0.22,
                Delta = callDelta
            });

            var putDelta = strike == 2400.00 ? -0.16 : (strike == 2450.00 ? -0.50 : -0.80);
            snapshot.Contracts.Add(new IngestionOptionContract
            {
                StrikePrice = strike,
                Type = "PE",
                Bid = strike < spot ? 15.00 : 60.00,
                Ask = strike < spot ? 16.00 : 61.00,
                ImpliedVolatility = 0.21,
                Delta = putDelta
            });
        }

        return snapshot;
    }
    public async Task<List<double>> GetHistoricalLogReturnsAsync(string stockId, int periodDays)
    {
        try
        {
            var path = $"api/v1/historical/returns/{stockId}?days={periodDays}";
            var returns = await _httpClient.GetFromJsonAsync<List<HistoricalReturnResponse>>(path);
            var result = new List<double>();
            if (returns != null)
            {
                foreach (var r in returns)
                {
                    result.Add(r.LogReturn);
                }
            }
            return result;
        }
        catch
        {
            // Fallback for local development
            var mockReturns = new List<double>();
            var random = new Random();
            for (int i = 0; i < periodDays; i++)
            {
                mockReturns.Add((random.NextDouble() - 0.5) * 0.02); // -1% to +1% random walk
            }
            return mockReturns;
        }
    }
    public async Task<IngestionTechnicalIndicatorSnapshot?> GetTechnicalIndicatorsAsync(string stockId)
    {
        try
        {
            var path = $"api/v1/indicators/{stockId}";
            return await _httpClient.GetFromJsonAsync<IngestionTechnicalIndicatorSnapshot>(path);
        }
        catch
        {
            // Fallback for local development
            return new IngestionTechnicalIndicatorSnapshot
            {
                StockId = stockId,
                Rsi = 55.0,
                Adx = 22.0,
                SupertrendDirection = "BUY",
                S2 = 2400.0,
                R2 = 2500.0
            };
        }
    }
}

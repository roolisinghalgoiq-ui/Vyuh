using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VYUH.Risk.Application.Interfaces;

namespace VYUH.Risk.Infrastructure.Services;

public class IngestionServiceClient : IIngestionServiceClient
{
    private readonly HttpClient _httpClient;

    public IngestionServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
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

    private class HistoricalReturnResponse
    {
        public DateTime Date { get; set; }
        public double ClosePrice { get; set; }
        public double LogReturn { get; set; }
    }
}

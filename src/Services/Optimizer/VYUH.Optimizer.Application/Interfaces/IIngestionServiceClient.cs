using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VYUH.Optimizer.Application.Interfaces;

public class IngestionOptionContract
{
    public double StrikePrice { get; set; }
    public string Type { get; set; } = string.Empty; // CE or PE
    public double Bid { get; set; }
    public double Ask { get; set; }
    public double ImpliedVolatility { get; set; }
    public double LiquidityScore { get; set; }
    public double Delta { get; set; }
    public double Gamma { get; set; }
    public double Vega { get; set; }
    public double Theta { get; set; }
}

public class IngestionOptionChainSnapshot
{
    public string StockId { get; set; } = string.Empty;
    public double SpotPrice { get; set; }
    public double FuturePrice { get; set; }
    public string ExpiryDate { get; set; } = string.Empty;
    public double IvPercentile { get; set; }
    public double MwplPct { get; set; }
    public string EarningsDate { get; set; } = string.Empty;
    public List<IngestionOptionContract> Contracts { get; set; } = new();
}

public class IngestionHistoricalRecord
{
    public DateTime Date { get; set; }
    public double ClosePrice { get; set; }
}

public class IngestionTechnicalIndicatorSnapshot
{
    public string StockId { get; set; } = string.Empty;
    public double Rsi { get; set; }
    public double Adx { get; set; }
    public string SupertrendDirection { get; set; } = string.Empty;
    public double S2 { get; set; }
    public double R2 { get; set; }
}

public interface IIngestionServiceClient
{
    Task<IngestionOptionChainSnapshot?> GetOptionChainAsync(string stockId, string expiryDate);
    Task<List<IngestionHistoricalRecord>> GetHistoricalPricesAsync(string stockId, int periodDays);

    Task<List<double>> GetHistoricalLogReturnsAsync(string stockId, int periodDays);

    Task<IngestionTechnicalIndicatorSnapshot?> GetTechnicalIndicatorsAsync(string stockId);
}

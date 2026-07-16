using System;

namespace VYUH.Optimizer.Domain;

public class StockScoreArchive
{
    public long ScoreId { get; set; }
    public string StockId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public double SpotPrice { get; set; }
    public double IvPercentile { get; set; }
    public double LiquidityScore { get; set; }
    public string StrategySelected { get; set; } = string.Empty;
    public double ExpectedYield { get; set; }
    public double RiskScore { get; set; } // Mapped to Hazard
}

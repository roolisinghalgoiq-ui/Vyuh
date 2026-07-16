using System;

namespace VYUH.Optimizer.Domain;

public class StrategySelectionResult
{
    public string StockId { get; set; } = string.Empty;
    public double SpotPrice { get; set; }
    public string ExpiryDate { get; set; } = string.Empty;
    public double IvPercentile { get; set; }
    public double Adx { get; set; }
    public string StrategySelected { get; set; } = string.Empty; // 'STRADDLE', 'STRANGLE', 'IRON_CONDOR', 'LONG_STRADDLE', 'NONE'
    public double AllocationPercentage { get; set; }
    public DateTime Timestamp { get; set; }
}

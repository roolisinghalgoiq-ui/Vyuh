using System;

namespace VYUH.Optimizer.Domain;

public class RangeProbabilityResult
{
    public string StockId { get; set; } = string.Empty;
    public double SpotPrice { get; set; }
    public string ExpiryDate { get; set; } = string.Empty;
    public double LowerStrikePrice { get; set; }
    public double UpperStrikePrice { get; set; }
    public double ImpliedVolatility { get; set; }
    public double RiskFreeRate { get; set; }
    public double ProbInRange { get; set; }
    public DateTime Timestamp { get; set; }
}

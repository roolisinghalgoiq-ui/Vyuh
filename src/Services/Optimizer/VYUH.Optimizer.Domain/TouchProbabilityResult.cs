using System;

namespace VYUH.Optimizer.Domain;

public class TouchProbabilityResult
{
    public string StockId { get; set; } = string.Empty;
    public double SpotPrice { get; set; }
    public string ExpiryDate { get; set; } = string.Empty;
    public double StrikePrice { get; set; }
    public double ImpliedVolatility { get; set; }
    public double RiskFreeRate { get; set; }
    public double ProbTouch { get; set; }
    public DateTime Timestamp { get; set; }
}

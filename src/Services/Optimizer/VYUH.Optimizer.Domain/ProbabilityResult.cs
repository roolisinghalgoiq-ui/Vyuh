using System;

namespace VYUH.Optimizer.Domain;

public class ProbabilityResult
{
    public string StockId { get; set; } = string.Empty;
    public double SpotPrice { get; set; }
    public string ExpiryDate { get; set; } = string.Empty;
    public double StrikePrice { get; set; }
    public double ProbAbove { get; set; }
    public double ProbBelow { get; set; }
    public int NumberOfSimulations { get; set; }
    public DateTime Timestamp { get; set; }
}

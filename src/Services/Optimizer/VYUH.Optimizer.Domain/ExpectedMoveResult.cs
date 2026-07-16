using System;

namespace VYUH.Optimizer.Domain;

public class ExpectedMoveResult
{
    public string StockId { get; set; } = string.Empty;
    public double SpotPrice { get; set; }
    public double ExpectedMoveIvBased { get; set; }
    public double ExpectedMoveStraddleBased { get; set; }
    public double UpperBoundary { get; set; }
    public double LowerBoundary { get; set; }
    public double Atr14 { get; set; }
    public DateTime Timestamp { get; set; }
}

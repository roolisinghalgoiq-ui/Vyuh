using System;

namespace VYUH.Risk.Domain;

public class ExitConfig
{
    public string StockId { get; set; } = string.Empty;
    public double StopLossMultiplier { get; set; } = 3.0; // 3x entry price
    public double TargetDecayPct { get; set; } = 80.0; // 80% decay (20% of entry price remaining)
    public DateTime UpdatedAt { get; set; }
}

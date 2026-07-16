using System;

namespace VYUH.Optimizer.Domain;

public class FilteringPipelineResult
{
    public string StockId { get; set; } = string.Empty;
    public bool IsPassed { get; set; }
    public string RejectedByFilter { get; set; } = string.Empty; // Empty if passed
    public DateTime Timestamp { get; set; }
}

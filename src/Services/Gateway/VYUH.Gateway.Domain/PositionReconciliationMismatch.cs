using System;

namespace VYUH.Gateway.Domain;

public class PositionReconciliationMismatch
{
    public Guid MismatchId { get; set; } = Guid.NewGuid();
    public string StockId { get; set; } = string.Empty;
    public string OptionType { get; set; } = string.Empty;
    public double StrikePrice { get; set; }
    public int LocalQty { get; set; }
    public int BrokerQty { get; set; }
    public string DiscrepancyType { get; set; } = string.Empty; // 'QUANTITY_MISMATCH', 'MISSING_BROKER_POSITION', 'EXCESS_BROKER_POSITION'
    public bool Resolved { get; set; }
    public DateTime DetectedAt { get; set; }
}

using System;
using System.Collections.Generic;

namespace VYUH.Ingestion.Domain;

public class OptionChainSnapshot
{
    public string StockId { get; set; } = string.Empty;
    public double SpotPrice { get; set; }
    public double FuturePrice { get; set; }
    public string ExpiryDate { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public double IvPercentile { get; set; }
    public List<OptionContract> Contracts { get; set; } = new();
}

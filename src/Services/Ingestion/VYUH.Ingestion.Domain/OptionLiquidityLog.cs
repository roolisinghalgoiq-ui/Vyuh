using System;

namespace VYUH.Ingestion.Domain;

public class OptionLiquidityLog
{
    public long LogId { get; set; }
    public string StockId { get; set; } = string.Empty;
    public double StrikePrice { get; set; }
    public string OptionType { get; set; } = string.Empty;
    public string ExpiryDate { get; set; } = string.Empty;
    public double LiquidityScore { get; set; }
    public DateTime Timestamp { get; set; }
}

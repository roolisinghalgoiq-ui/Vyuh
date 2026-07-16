using System;

namespace VYUH.Risk.Domain;

public class MarginEstimationResult
{
    public string StockId { get; set; } = string.Empty;
    public string ExpiryDate { get; set; } = string.Empty;
    public double SpotPrice { get; set; }
    public double StrikePrice { get; set; }
    public string OptionType { get; set; } = string.Empty; // 'CE' or 'PE'
    public double SpanMargin { get; set; }
    public double ExposureMargin { get; set; }
    public double TotalMargin { get; set; }
    public DateTime Timestamp { get; set; }
}

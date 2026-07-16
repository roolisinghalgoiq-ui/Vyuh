using System;
using System.Collections.Generic;

namespace VYUH.Ingestion.Domain;

public class TechnicalIndicatorSnapshot
{
    public string StockId { get; set; } = string.Empty;
    public double Rsi { get; set; }
    public double Ema20 { get; set; }
    public double Ema50 { get; set; }
    public double Ema200 { get; set; }
    public double Sma20 { get; set; }
    public double Sma50 { get; set; }
    public double Vwap { get; set; }
    public double Adx { get; set; }
    public double MacdLine { get; set; }
    public double SignalLine { get; set; }
    public string SupertrendDirection { get; set; } = string.Empty; // BUY or SELL
    public double SupertrendValue { get; set; }
    public List<double> SupportLevels { get; set; } = new();
    public List<double> ResistanceLevels { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

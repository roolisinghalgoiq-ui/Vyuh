using System;

namespace VYUH.Optimizer.Domain;

public class StrikeSelectionResult
{
    public string StockId { get; set; } = string.Empty;
    public string ExpiryDate { get; set; } = string.Empty;
    public double SelectedCallStrike { get; set; }
    public double SelectedCallDelta { get; set; }
    public double SelectedPutStrike { get; set; }
    public double SelectedPutDelta { get; set; }
    public double SupportS2 { get; set; }
    public double ResistanceR2 { get; set; }
    public DateTime Timestamp { get; set; }
}

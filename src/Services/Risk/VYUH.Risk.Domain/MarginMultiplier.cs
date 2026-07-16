namespace VYUH.Risk.Domain;

public class MarginMultiplier
{
    public string StockId { get; set; } = string.Empty;
    public double MultiplierValue { get; set; } = 1.0;
    public double ExposureMarginPct { get; set; } = 0.03; // Default 3%
    public double VolShiftPct { get; set; } = 0.10; // Default 10%
    public string Description { get; set; } = string.Empty;
}

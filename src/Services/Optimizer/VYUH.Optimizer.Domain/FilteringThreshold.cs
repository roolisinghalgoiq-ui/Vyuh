namespace VYUH.Optimizer.Domain;

public class FilteringThreshold
{
    public string FilterId { get; set; } = string.Empty;
    public double ThresholdValue { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string Description { get; set; } = string.Empty;
}

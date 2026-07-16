namespace VYUH.Risk.Domain;

public class PositionExitEvaluation
{
    public string PositionId { get; set; } = string.Empty;
    public string StockId { get; set; } = string.Empty;
    public double EntryPremium { get; set; }
    public double CurrentPremium { get; set; }
    public bool IsExitTriggered { get; set; }
    public string ExitReason { get; set; } = "NONE"; // 'STOP_LOSS', 'PROFIT_TARGET', 'NONE'
}

using System;

namespace VYUH.Risk.Domain;

public class VarAuditLog
{
    public Guid LogId { get; set; } = Guid.NewGuid();
    public double PortfolioValue { get; set; }
    public double ComputedVar99 { get; set; }
    public DateTime CalculatedAt { get; set; }
}

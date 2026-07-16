using System;

namespace VYUH.Ingestion.Domain;

public class UnderlyingStock
{
    public string StockId { get; set; } = string.Empty;
    public string StockName { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public int LotSize { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

using System;

namespace VYUH.Ingestion.Domain;

public class HistoricalReturn
{
    public DateTime Date { get; set; }
    public double ClosePrice { get; set; }
    public double LogReturn { get; set; }
}

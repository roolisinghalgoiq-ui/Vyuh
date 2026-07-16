using System;

namespace VYUH.Gateway.Domain;

public class OrderExecutionLog
{
    public Guid OrderId { get; set; } = Guid.NewGuid();
    public string StockId { get; set; } = string.Empty;
    public string OptionType { get; set; } = string.Empty;
    public double StrikePrice { get; set; }
    public string Action { get; set; } = string.Empty; // 'SELL' or 'BUY'
    public int Quantity { get; set; }
    public double ExecutionPrice { get; set; }
    public string Status { get; set; } = "PENDING"; // 'PENDING', 'FILLED', 'REJECTED'
    public string BrokerOrderId { get; set; } = string.Empty;
    public DateTime ExecutedAt { get; set; }
}

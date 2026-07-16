using System;

namespace VYUH.Gateway.Application;

public class BrokerOrderRequest
{
    public string StockId { get; set; } = string.Empty;
    public string OptionType { get; set; } = string.Empty; // 'CE' or 'PE'
    public double StrikePrice { get; set; }
    public string Action { get; set; } = string.Empty; // 'SELL' or 'BUY'
    public int Quantity { get; set; }
    public double Price { get; set; }
}

public class BrokerOrderResponse
{
    public bool Success { get; set; }
    public string BrokerOrderId { get; set; } = string.Empty;
    public double FilledPrice { get; set; }
    public string Message { get; set; } = string.Empty;
}


public class BrokerPositionResponse
{
    public string StockId { get; set; } = string.Empty;
    public string OptionType { get; set; } = string.Empty; // 'CE' or 'PE'
    public double StrikePrice { get; set; }
    public int Quantity { get; set; }
    public double AveragePrice { get; set; }
}

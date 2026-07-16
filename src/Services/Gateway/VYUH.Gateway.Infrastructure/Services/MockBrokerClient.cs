using System;
using System.Threading.Tasks;
using VYUH.Gateway.Application;

namespace VYUH.Gateway.Infrastructure.Services;

public class MockBrokerClient : IBrokerClient
{
    public Task<System.Collections.Generic.List<BrokerPositionResponse>> GetBrokerActivePositionsAsync()
    {
        var positions = new System.Collections.Generic.List<BrokerPositionResponse>
        {
            new BrokerPositionResponse { StockId = "RELIANCE", OptionType = "PE", StrikePrice = 2300.0, Quantity = -250, AveragePrice = 12.50 },
            new BrokerPositionResponse { StockId = "INFY", OptionType = "CE", StrikePrice = 1600.0, Quantity = -500, AveragePrice = 15.00 }
        };
        return Task.FromResult(positions);
    }

    public Task<BrokerOrderResponse> SubmitOrderAsync(BrokerOrderRequest request)
    {
        // Mock broker execution: dispatches in < 1ms, fills at requested price
        var response = new BrokerOrderResponse
        {
            Success = true,
            BrokerOrderId = "BRK_" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper(),
            FilledPrice = request.Price,
            Message = "Order executed successfully on broker exchange sandbox."
        };
        return Task.FromResult(response);
    }
}

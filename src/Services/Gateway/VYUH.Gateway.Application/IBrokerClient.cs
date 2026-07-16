using System.Threading.Tasks;

namespace VYUH.Gateway.Application;

public interface IBrokerClient
{
    Task<BrokerOrderResponse> SubmitOrderAsync(BrokerOrderRequest request);
    Task<System.Collections.Generic.List<BrokerPositionResponse>> GetBrokerActivePositionsAsync();
}

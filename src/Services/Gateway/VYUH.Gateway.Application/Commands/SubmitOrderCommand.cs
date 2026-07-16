using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using VYUH.Gateway.Domain;

namespace VYUH.Gateway.Application.Commands;

public record SubmitOrderCommand(
    string StockId, 
    string OptionType, 
    double StrikePrice, 
    string Action, 
    int Quantity, 
    double Price) : IRequest<OrderExecutionLog>;

public class SubmitOrderCommandHandler : IRequestHandler<SubmitOrderCommand, OrderExecutionLog>
{
    private readonly IBrokerClient _brokerClient;
    private readonly IOrderExecutionLogRepository _repository;

    public SubmitOrderCommandHandler(IBrokerClient brokerClient, IOrderExecutionLogRepository repository)
    {
        _brokerClient = brokerClient;
        _repository = repository;
    }

    public async Task<OrderExecutionLog> Handle(SubmitOrderCommand request, CancellationToken cancellationToken)
    {
        // 1. Submit order to broker
        var brokerReq = new BrokerOrderRequest
        {
            StockId = request.StockId,
            OptionType = request.OptionType,
            StrikePrice = request.StrikePrice,
            Action = request.Action,
            Quantity = request.Quantity,
            Price = request.Price
        };

        var brokerResp = await _brokerClient.SubmitOrderAsync(brokerReq);

        // 2. Log execution details to PostgreSQL
        var log = new OrderExecutionLog
        {
            OrderId = Guid.NewGuid(),
            StockId = request.StockId,
            OptionType = request.OptionType,
            StrikePrice = request.StrikePrice,
            Action = request.Action,
            Quantity = request.Quantity,
            ExecutionPrice = brokerResp.Success ? brokerResp.FilledPrice : 0.0,
            Status = brokerResp.Success ? "FILLED" : "REJECTED",
            BrokerOrderId = brokerResp.BrokerOrderId,
            ExecutedAt = DateTime.UtcNow
        };

        await _repository.AddLogAsync(log);

        return log;
    }
}

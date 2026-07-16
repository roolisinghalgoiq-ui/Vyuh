using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VYUH.Ingestion.Domain;

namespace VYUH.Ingestion.Infrastructure;

public class KafkaOptionChainConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<KafkaOptionChainConsumer> _logger;
    private readonly string _topic = "raw.options.chain.tick";
    private readonly string _bootstrapServers = "localhost:9092";

    public KafkaOptionChainConsumer(IServiceProvider serviceProvider, ILogger<KafkaOptionChainConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka Ingestion Consumer Service Started.");
        await Task.Yield();

        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = "vyuh-ingestion-group",
            AutoOffsetReset = AutoOffsetReset.Latest,
            EnableAutoCommit = true
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(_topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(TimeSpan.FromMilliseconds(500));
                if (result != null)
                {
                    var json = result.Message.Value;
                    var snapshot = JsonSerializer.Deserialize<OptionChainSnapshot>(json);
                    if (snapshot != null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        await mediator.Send(new Application.UpdateOptionChainCommand(snapshot), stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unable to consume options chain tick from Kafka. Retrying in 5 seconds...");
                await Task.Delay(5000, stoppingToken);
            }
        }

        consumer.Close();
    }
}

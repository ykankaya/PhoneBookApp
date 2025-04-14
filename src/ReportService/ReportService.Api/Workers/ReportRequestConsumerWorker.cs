using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Options;
using ReportService.Application.Configuration;
using ReportService.Application.Features.Reports.Commands.ProcessReport;

namespace ReportService.Api.Workers
{
  public class ReportRequestConsumerWorker : BackgroundService
    {
        private readonly ILogger<ReportRequestConsumerWorker> _logger;
        private readonly KafkaOptions _kafkaOptions;
        private readonly IServiceScopeFactory _scopeFactory; 

        public ReportRequestConsumerWorker(
            IOptions<KafkaOptions> kafkaOptions,
            ILogger<ReportRequestConsumerWorker> logger,
            IServiceScopeFactory scopeFactory)
        {
            _kafkaOptions = kafkaOptions.Value;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Kafka Consumer Worker starting.");

            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaOptions.BootstrapServers,
                GroupId = "report-processor-group", 
                AutoOffsetReset = AutoOffsetReset.Earliest, 
                EnableAutoCommit = false
            };

           
            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

            consumer.Subscribe(_kafkaOptions.ReportRequestTopic);
            _logger.LogInformation("Subscribed to Kafka topic: {Topic}", _kafkaOptions.ReportRequestTopic);


            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                     
                        var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1)); 

                        if (consumeResult == null) 
                        {
                            await Task.Delay(100, stoppingToken); 
                            continue;
                        }

                        _logger.LogInformation("Received message: {MessageValue} from topic {TopicPartitionOffset}",
                            consumeResult.Message.Value, consumeResult.TopicPartitionOffset);

                        if (Guid.TryParse(consumeResult.Message.Value, out Guid reportId))
                        {
                         
                            using (var scope = _scopeFactory.CreateScope())
                            {
                                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                                try
                                {
                                
                                    await mediator.Send(new ProcessReportCommand(reportId), stoppingToken);

                                 
                                    consumer.Commit(consumeResult);
                                     _logger.LogInformation("Message processed and committed for ReportId: {ReportId}", reportId);
                                }
                                catch (Exception ex)
                                {
                                
                                    _logger.LogError(ex, "Error processing message for ReportId: {ReportId}. Message will not be committed and might be retried.", reportId);
                                  
                                }
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Received invalid message (not a valid Guid): {MessageValue}. Skipping.", consumeResult.Message.Value);
                           
                            consumer.Commit(consumeResult);
                        }
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError(e, "Kafka Consume error");
                      
                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                   
                        break;
                    }
                     catch (Exception ex) 
                    {
                         _logger.LogError(ex, "Unexpected error in Kafka Consumer Worker loop.");
                         await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); 
                    }
                }
            }
            finally
            {
              
                consumer.Close();
                 _logger.LogInformation("Kafka Consumer Worker stopped.");
            }
        }
    }
}

using Confluent.Kafka;
using Microsoft.Extensions.Options;
using ReportService.Application.Configuration;
using ReportService.Application.Interfaces.Infrastructure;

namespace ReportService.Infrastructure.Messaging
{


  public class KafkaProducer : IKafkaProducer
  {
    private readonly IProducer<Null, string> _producer;
    private readonly KafkaOptions _options;

    public KafkaProducer(IOptions<KafkaOptions> options)
    {
      _options = options.Value;
      var config = new ProducerConfig { BootstrapServers = _options.BootstrapServers };
   
      _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task ProduceAsync(string topic, string message, CancellationToken cancellationToken = default)
    {
      try
      {
       
        var deliveryResult = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message }, cancellationToken);
        
        Console.WriteLine($"Kafka delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'");
      }
      catch (ProduceException<Null, string> e)
      {
        Console.WriteLine($"Kafka delivery failed: {e.Error.Reason}");
        // TODO: Hata yönetimi (loglama, tekrar deneme mekanizması?)
        throw; 
      }
    }

    public void Dispose()
    {
     
      _producer.Flush(TimeSpan.FromSeconds(10)); 
      _producer.Dispose();
      GC.SuppressFinalize(this);
    }
  }
}

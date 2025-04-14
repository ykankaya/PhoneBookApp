namespace ReportService.Application.Interfaces.Infrastructure
{
  public interface IKafkaProducer : IDisposable 
  {
    Task ProduceAsync(string topic, string message, CancellationToken cancellationToken = default);
  }
}

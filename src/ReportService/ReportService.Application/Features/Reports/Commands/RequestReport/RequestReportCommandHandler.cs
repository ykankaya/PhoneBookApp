using MediatR;
using ReportService.Application.Interfaces.Persistence;
using ReportService.Domain.Entities;
using ReportService.Domain.Enums;
using Microsoft.Extensions.Options;
using ReportService.Application.Configuration;
using ReportService.Application.Interfaces.Infrastructure;
namespace ReportService.Application.Features.Reports.Commands.RequestReport
{
  public class RequestReportCommandHandler : IRequestHandler<RequestReportCommand, Guid>
  {
    private readonly IReportDbContext _context;
    private readonly IKafkaProducer _producer; 
    private readonly KafkaOptions _kafkaOptions; 

    public RequestReportCommandHandler(IReportDbContext context, IKafkaProducer producer, IOptions<KafkaOptions> kafkaOptions)
    {
      _context = context;
      _producer = producer;
      _kafkaOptions = kafkaOptions.Value; 
    }

    public async Task<Guid> Handle(RequestReportCommand request, CancellationToken cancellationToken)
    {
      var newReport = new Report
      {
        Id = Guid.NewGuid(),
        TalepEdildigiTarih = DateTime.UtcNow,
        Durum = ReportStatus.Hazirlaniyor
      };

      await _context.Reports.AddAsync(newReport, cancellationToken);
      await _context.SaveChangesAsync(cancellationToken); 
      
      try
      {
  
        await _producer.ProduceAsync(_kafkaOptions.ReportRequestTopic, newReport.Id.ToString(), cancellationToken);
        Console.WriteLine($"Kafka'ya olay gönderildi: ReportRequested, ReportId: {newReport.Id}");
      }
      catch (Exception ex)
      {
      
        Console.WriteLine($"Kafka'ya mesaj gönderilirken hata oluştu: {ex.Message}");

      }


      return newReport.Id;
    }
  }
}

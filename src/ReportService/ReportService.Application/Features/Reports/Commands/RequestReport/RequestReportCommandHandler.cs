using MediatR;
using ReportService.Application.Interfaces.Persistence;
using ReportService.Domain.Entities;
using ReportService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Application.Features.Reports.Commands.RequestReport
{
  public class RequestReportCommandHandler : IRequestHandler<RequestReportCommand, Guid>
  {
    private readonly IReportDbContext _context;
    // TODO: Kafka Producer'ı enjekte et (örnek: IKafkaProducer _producer;)

    public RequestReportCommandHandler(IReportDbContext context /*, IKafkaProducer producer */)
    {
      _context = context;
      // _producer = producer;
    }

    public async Task<Guid> Handle(RequestReportCommand request, CancellationToken cancellationToken)
    {
   
      var newReport = new Report
      {
        Id = Guid.NewGuid(),
        TalepEdildigiTarih = DateTime.UtcNow,
        Durum = ReportStatus.Hazirlaniyor
      };

      // 2. Veritabanına kaydet
      await _context.Reports.AddAsync(newReport, cancellationToken);
      await _context.SaveChangesAsync(cancellationToken);

      // 3. Kafka'ya rapor talebi olayını gönder (Report ID ile birlikte)
      // Gerçek Kafka Producer implementasyonu burada çağrılacak.
      // Şimdilik loglama veya simülasyon yapabiliriz.
      Console.WriteLine($"Kafka'ya gönderilecek olay: ReportRequested, ReportId: {newReport.Id}");
      // await _producer.ProduceAsync("report-requests", newReport.Id.ToString(), cancellationToken); // Gerçek implementasyon

      return newReport.Id;
    }
  }
}

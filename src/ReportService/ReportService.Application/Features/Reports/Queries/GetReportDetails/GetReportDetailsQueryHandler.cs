using MediatR;
using Microsoft.EntityFrameworkCore;
using ReportService.Application.Features.Reports.DTOs;
using ReportService.Application.Interfaces.Persistence;
using ReportService.Domain.Enums;

namespace ReportService.Application.Features.Reports.Queries.GetReportDetails
{
  public class GetReportDetailsQueryHandler : IRequestHandler<GetReportDetailsQuery, ReportDetailDto?>
  {
    private readonly IReportDbContext _context;

    public GetReportDetailsQueryHandler(IReportDbContext context)
    {
      _context = context;
    }

    public async Task<ReportDetailDto?> Handle(GetReportDetailsQuery request, CancellationToken cancellationToken)
    {
      var reportQuery = _context.Reports.AsNoTracking();


      var report = await reportQuery
                           .Include(r => r.RaporDetaylari)
                           .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);


      if (report == null)
      {
        return null;
      }

      var reportDetailDto = new ReportDetailDto
      {
        Id = report.Id,
        TalepEdildigiTarih = report.TalepEdildigiTarih,
        Durum = report.Durum
      };

    
      if (report.Durum == ReportStatus.Tamamlandi && report.RaporDetaylari != null)
      {
        reportDetailDto.Istatistikler = report.RaporDetaylari.Select(rd => new ReportStatisticsDto
        {
          Id = rd.Id,
          KonumBilgisi = rd.KonumBilgisi,
          KisiSayisi = rd.KisiSayisi,
          TelefonNumarasiSayisi = rd.TelefonNumarasiSayisi
        }).ToList();
      }

      return reportDetailDto;
    }
  }
}

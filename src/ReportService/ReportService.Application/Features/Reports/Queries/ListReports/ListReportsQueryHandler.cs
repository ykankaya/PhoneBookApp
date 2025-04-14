using MediatR;
using Microsoft.EntityFrameworkCore;
using ReportService.Application.Features.Reports.DTOs;
using ReportService.Application.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Application.Features.Reports.Queries.ListReports
{
  public class ListReportsQueryHandler : IRequestHandler<ListReportsQuery, List<ReportSummaryDto>>
  {
    private readonly IReportDbContext _context;

    public ListReportsQueryHandler(IReportDbContext context)
    {
      _context = context;
    }

    public async Task<List<ReportSummaryDto>> Handle(ListReportsQuery request, CancellationToken cancellationToken)
    {
      var reports = await _context.Reports
                                 .AsNoTracking()
                                 .OrderByDescending(r => r.TalepEdildigiTarih) 
                                 .Select(r => new ReportSummaryDto
                                 {
                                   Id = r.Id,
                                   TalepEdildigiTarih = r.TalepEdildigiTarih,
                                   Durum = r.Durum
                                 })
                                 .ToListAsync(cancellationToken);
      return reports;
    }
  }
}

using Microsoft.EntityFrameworkCore;
using ReportService.Domain.Entities;

namespace ReportService.Application.Interfaces.Persistence
{
  public interface IReportDbContext
  {
    DbSet<Report> Reports { get; }
    DbSet<ReportDetail> ReportDetails { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
  }
}

using Microsoft.EntityFrameworkCore;
using ReportService.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReportService.Application.Interfaces.Persistence
{
  public interface IReportDbContext
  {
    DbSet<Report> Reports { get; }
    DbSet<ReportDetail> ReportDetails { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
  }
}

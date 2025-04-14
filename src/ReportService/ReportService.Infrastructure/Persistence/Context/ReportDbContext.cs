using Microsoft.EntityFrameworkCore;
using ReportService.Application.Interfaces.Persistence;
using ReportService.Domain.Entities;
using System.Reflection;

namespace ReportService.Infrastructure.Persistence.Context
{
  public class ReportDbContext : DbContext, IReportDbContext
  {
    public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options)
    {
    }

    public DbSet<Report> Reports => Set<Report>();
    public DbSet<ReportDetail> ReportDetails => Set<ReportDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
      base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
      return base.SaveChangesAsync(cancellationToken);
    }
  }
}

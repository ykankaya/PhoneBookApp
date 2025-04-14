using ReportService.Domain.Enums;

namespace ReportService.Application.Features.Reports.DTOs
{
  public class ReportSummaryDto
  {
    public Guid Id { get; set; }
    public DateTime TalepEdildigiTarih { get; set; }
    public ReportStatus Durum { get; set; }
  }
}

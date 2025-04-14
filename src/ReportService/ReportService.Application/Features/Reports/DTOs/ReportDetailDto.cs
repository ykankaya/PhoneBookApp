using ReportService.Domain.Enums;

namespace ReportService.Application.Features.Reports.DTOs
{
  public class ReportDetailDto
  {
    public Guid Id { get; set; }
    public DateTime TalepEdildigiTarih { get; set; }
    public ReportStatus Durum { get; set; }
    public List<ReportStatisticsDto>? Istatistikler { get; set; } 
  }
}

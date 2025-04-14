namespace ReportService.Application.Features.Reports.DTOs
{
  public class ReportStatisticsDto
  {
    public Guid Id { get; set; }
    public string KonumBilgisi { get; set; } = string.Empty;
    public int KisiSayisi { get; set; }
    public int TelefonNumarasiSayisi { get; set; }
  }
}

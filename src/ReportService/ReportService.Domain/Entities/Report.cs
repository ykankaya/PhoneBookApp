using ReportService.Domain.Enums;


namespace ReportService.Domain.Entities
{
  public class Report
  {
    public Guid Id { get; set; }
    public DateTime TalepEdildigiTarih { get; set; }
    public ReportStatus Durum { get; set; }


    public virtual ICollection<ReportDetail> RaporDetaylari { get; set; } = new List<ReportDetail>();
  }
}

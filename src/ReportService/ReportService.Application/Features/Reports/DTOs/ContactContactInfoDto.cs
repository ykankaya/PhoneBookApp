namespace ReportService.Application.Features.Reports.DTOs;
public class ContactContactInfoDto
{
    public Guid Id { get; set; }
    public string BilgiTipi { get; set; } = string.Empty;
    public string BilgiIcerigi { get; set; } = string.Empty;
}
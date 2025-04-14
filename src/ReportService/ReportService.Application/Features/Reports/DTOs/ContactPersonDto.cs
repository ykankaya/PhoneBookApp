namespace ReportService.Application.Features.Reports.DTOs;

public class ContactPersonDto
{
    public Guid Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string Firma { get; set; } = string.Empty;
    public List<ContactContactInfoDto> IletisimBilgileri { get; set; } = new List<ContactContactInfoDto>();
}
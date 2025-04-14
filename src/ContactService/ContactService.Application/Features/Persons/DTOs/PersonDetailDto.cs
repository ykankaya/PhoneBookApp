using ContactService.Application.Features.ContactInfos.DTOs;

namespace ContactService.Application.Features.Persons.DTOs
{
  public class PersonDetailDto
  {
    public Guid Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string Firma { get; set; } = string.Empty;
    public List<ContactInfoDto> IletisimBilgileri { get; set; } = new List<ContactInfoDto>();
  }
}

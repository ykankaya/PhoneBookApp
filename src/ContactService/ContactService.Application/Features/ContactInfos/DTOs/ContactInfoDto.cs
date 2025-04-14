using ContactService.Domain.Enums;

namespace ContactService.Application.Features.ContactInfos.DTOs
{
  public class ContactInfoDto
  {
    public Guid Id { get; set; }
    public ContactType BilgiTipi { get; set; }
    public string BilgiIcerigi { get; set; } = string.Empty;
  }
}

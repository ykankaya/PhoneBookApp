using ContactService.Domain.Enums;
using System;

namespace ContactService.Domain.Entities
{
  public class ContactInfo
  {
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public ContactType BilgiTipi { get; set; }
    public string BilgiIcerigi { get; set; } = string.Empty;


    public virtual Person Person { get; set; } = null!;
  }
}

using ContactService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactService.Application.Features.ContactInfos.DTOs
{
  public class ContactInfoDto
  {
    public Guid Id { get; set; }
    public ContactType BilgiTipi { get; set; }
    public string BilgiIcerigi { get; set; } = string.Empty;
  }
}

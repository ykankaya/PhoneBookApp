using ContactService.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactService.Application.Features.ContactInfos.Commands.AddContactInfo
{
  public class AddContactInfoCommand : IRequest<Guid>
  {
    public Guid PersonId { get; set; }
    public ContactType BilgiTipi { get; set; }
    public string BilgiIcerigi { get; set; } = string.Empty;
  }
}

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactService.Application.Features.ContactInfos.Commands.RemoveContactInfo
{
  public class RemoveContactInfoCommand : IRequest<bool> 
  {
 
    public Guid PersonId { get; set; }
    public Guid ContactInfoId { get; set; }

    public RemoveContactInfoCommand(Guid personId, Guid contactInfoId)
    {
      PersonId = personId;
      ContactInfoId = contactInfoId;
    }
  }
}

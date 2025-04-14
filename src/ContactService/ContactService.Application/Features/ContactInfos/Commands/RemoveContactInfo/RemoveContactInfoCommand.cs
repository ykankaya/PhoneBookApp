using MediatR;

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

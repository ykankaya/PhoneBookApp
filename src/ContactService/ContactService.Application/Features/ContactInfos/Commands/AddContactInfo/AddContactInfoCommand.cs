using ContactService.Domain.Enums;
using MediatR;

namespace ContactService.Application.Features.ContactInfos.Commands.AddContactInfo
{
  public class AddContactInfoCommand : IRequest<Guid>
  {
    public Guid PersonId { get; set; }
    public ContactType BilgiTipi { get; set; }
    public string BilgiIcerigi { get; set; } = string.Empty;
  }
}

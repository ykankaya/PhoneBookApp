using ContactService.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContactService.Application.Features.ContactInfos.Commands.RemoveContactInfo
{
  public class RemoveContactInfoCommandHandler : IRequestHandler<RemoveContactInfoCommand, bool>
  {
    private readonly IContactDbContext _context;

    public RemoveContactInfoCommandHandler(IContactDbContext context)
    {
      _context = context;
    }

    public async Task<bool> Handle(RemoveContactInfoCommand request, CancellationToken cancellationToken)
    {
     
      var contactInfoToRemove = await _context.ContactInfos
                                            .FirstOrDefaultAsync(ci => ci.Id == request.ContactInfoId && ci.PersonId == request.PersonId, cancellationToken);

      if (contactInfoToRemove == null)
      {
        return false; 
      }

      _context.ContactInfos.Remove(contactInfoToRemove);
      var result = await _context.SaveChangesAsync(cancellationToken);

      return result > 0;
    }
  }
}

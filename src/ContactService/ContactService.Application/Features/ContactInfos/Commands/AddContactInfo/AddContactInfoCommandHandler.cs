using ContactService.Application.Interfaces.Persistence;
using ContactService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactService.Application.Features.ContactInfos.Commands.AddContactInfo
{
  public class AddContactInfoCommandHandler : IRequestHandler<AddContactInfoCommand, Guid>
  {
    private readonly IContactDbContext _context;

    public AddContactInfoCommandHandler(IContactDbContext context)
    {
      _context = context;
    }

    public async Task<Guid> Handle(AddContactInfoCommand request, CancellationToken cancellationToken)
    {
   
      var personExists = await _context.Persons.AnyAsync(p => p.Id == request.PersonId, cancellationToken);
      if (!personExists)
      {

        throw new Exception($"Person with Id {request.PersonId} not found."); 
      }

      var newContactInfo = new ContactInfo
      {
        Id = Guid.NewGuid(),
        PersonId = request.PersonId,
        BilgiTipi = request.BilgiTipi,
        BilgiIcerigi = request.BilgiIcerigi
      };

      await _context.ContactInfos.AddAsync(newContactInfo, cancellationToken);
      await _context.SaveChangesAsync(cancellationToken);

      return newContactInfo.Id;
    }
  }
}

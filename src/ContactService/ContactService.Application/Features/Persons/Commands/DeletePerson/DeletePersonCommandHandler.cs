using ContactService.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactService.Application.Features.Persons.Commands.DeletePerson
{
  public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand, bool>
  {
    private readonly IContactDbContext _context;

    public DeletePersonCommandHandler(IContactDbContext context)
    {
      _context = context;
    }

    public async Task<bool> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
    {
      var personToDelete = await _context.Persons
                                        .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

      if (personToDelete == null)
      {
      
        return false;
      }

      _context.Persons.Remove(personToDelete);


      var result = await _context.SaveChangesAsync(cancellationToken);

      return result > 0; 
    }
  }
}

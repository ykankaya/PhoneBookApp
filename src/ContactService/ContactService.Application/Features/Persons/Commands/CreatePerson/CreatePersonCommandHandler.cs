using ContactService.Application.Interfaces.Persistence;
using ContactService.Domain.Entities;
using MediatR;

namespace ContactService.Application.Features.Persons.Commands.CreatePerson
{
  public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Guid>
  {
    private readonly IContactDbContext _context;

    public CreatePersonCommandHandler(IContactDbContext context )
    {
      _context = context;
    }

    public async Task<Guid> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {

      var person = new Person
      {
        Id = Guid.NewGuid(), 
        Ad = request.Ad,
        Soyad = request.Soyad,
        Firma = request.Firma
       
      };


      await _context.Persons.AddAsync(person, cancellationToken);


      await _context.SaveChangesAsync(cancellationToken);


      return person.Id;
    }
  }
}

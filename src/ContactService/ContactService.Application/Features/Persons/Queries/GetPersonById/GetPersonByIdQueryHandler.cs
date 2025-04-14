using ContactService.Application.Features.ContactInfos.DTOs;
using ContactService.Application.Features.Persons.DTOs;
using ContactService.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContactService.Application.Features.Persons.Queries.GetPersonById
{
  public class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, PersonDetailDto?>
  {
    private readonly IContactDbContext _context;

    public GetPersonByIdQueryHandler(IContactDbContext context)
    {
      _context = context;
    }

    public async Task<PersonDetailDto?> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
      var person = await _context.Persons
                                .AsNoTracking()
                                .Include(p => p.IletisimBilgileri) 
                                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

      if (person == null)
      {
        return null;
      }


      var personDetailDto = new PersonDetailDto
      {
        Id = person.Id,
        Ad = person.Ad,
        Soyad = person.Soyad,
        Firma = person.Firma,
        IletisimBilgileri = person.IletisimBilgileri.Select(ci => new ContactInfoDto
        {
          Id = ci.Id,
          BilgiTipi = ci.BilgiTipi,
          BilgiIcerigi = ci.BilgiIcerigi
        }).ToList()
      };

      return personDetailDto;
    }
  }
}

using ContactService.Application.Features.Persons.DTOs;
using ContactService.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactService.Application.Features.Persons.Queries.GetAllPersons
{
  public class GetAllPersonsQueryHandler : IRequestHandler<GetAllPersonsQuery, List<PersonDto>>
  {
    private readonly IContactDbContext _context;


    public GetAllPersonsQueryHandler(IContactDbContext context)
    {
      _context = context;
    }

    public async Task<List<PersonDto>> Handle(GetAllPersonsQuery request, CancellationToken cancellationToken)
    {
      var persons = await _context.Persons
                                  .AsNoTracking()
                                  .OrderBy(p => p.Ad) 
                                  .ToListAsync(cancellationToken);

 
      var personDtos = persons.Select(p => new PersonDto
      {
        Id = p.Id,
        Ad = p.Ad,
        Soyad = p.Soyad,
        Firma = p.Firma
      }).ToList();


      return personDtos;
    }
  }
}

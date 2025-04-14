using ContactService.Application.Features.Persons.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactService.Application.Features.Persons.Queries.GetPersonById
{
  public class GetPersonByIdQuery : IRequest<PersonDetailDto?> 
  {
    public Guid Id { get; set; }

    public GetPersonByIdQuery(Guid id)
    {
      Id = id;
    }
  }
}

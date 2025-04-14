using ContactService.Application.Features.Persons.DTOs;
using MediatR;

namespace ContactService.Application.Features.Persons.Queries.GetAllPersons
{
  public class GetAllPersonsQuery : IRequest<List<PersonDto>>
  {

  }
}

using ContactService.Application.Features.Persons.DTOs;
using MediatR;

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

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactService.Application.Features.Persons.Commands.DeletePerson
{
  public class DeletePersonCommand : IRequest<bool> 
  {
    public Guid Id { get; set; }

    public DeletePersonCommand(Guid id)
    {
      Id = id;
    }
  }
}

using MediatR;

namespace ContactService.Application.Features.Persons.Commands.CreatePerson
{
  public class CreatePersonCommand : IRequest<Guid>
  {
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string Firma { get; set; } = string.Empty;
  }
}

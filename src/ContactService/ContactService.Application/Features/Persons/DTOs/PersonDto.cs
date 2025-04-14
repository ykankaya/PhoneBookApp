namespace ContactService.Application.Features.Persons.DTOs
{
  public class PersonDto
  {
    public Guid Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string Firma { get; set; } = string.Empty;
  }
}

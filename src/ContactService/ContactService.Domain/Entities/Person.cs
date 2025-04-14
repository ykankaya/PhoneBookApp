namespace ContactService.Domain.Entities
{
  public class Person
  {
    public Guid Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string Firma { get; set; } = string.Empty;

    public virtual ICollection<ContactInfo> IletisimBilgileri { get; set; } = new List<ContactInfo>();
  }
}

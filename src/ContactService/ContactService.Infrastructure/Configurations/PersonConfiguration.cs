using ContactService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactService.Infrastructure.Configurations
{
  public class PersonConfiguration : IEntityTypeConfiguration<Person>
  {
    public void Configure(EntityTypeBuilder<Person> builder)
    {
      builder.HasKey(p => p.Id);

      builder.Property(p => p.Ad)
          .IsRequired()
          .HasMaxLength(100);

      builder.Property(p => p.Soyad)
          .IsRequired()
          .HasMaxLength(100);

      builder.Property(p => p.Firma)
          .HasMaxLength(150);

      // Person ile ContactInfo arasındaki one-to-many ilişki
      builder.HasMany(p => p.IletisimBilgileri)
             .WithOne(ci => ci.Person)       // ContactInfo'daki Person navigation property'si
             .HasForeignKey(ci => ci.PersonId) // ContactInfo'daki Foreign Key
             .OnDelete(DeleteBehavior.Cascade); // Kişi silinince iletişim bilgileri de silinsin
    }
  }
}

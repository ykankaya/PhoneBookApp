using ContactService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace ContactService.Infrastructure.Configurations
{
  public class ContactInfoConfiguration : IEntityTypeConfiguration<ContactInfo>
  {
    public void Configure(EntityTypeBuilder<ContactInfo> builder)
    {
      builder.HasKey(ci => ci.Id);

      builder.Property(ci => ci.BilgiIcerigi)
          .IsRequired()
          .HasMaxLength(200);

      builder.Property(ci => ci.BilgiTipi)
          .IsRequired()
          .HasConversion<string>()
          .HasMaxLength(50);

    }
  }
}

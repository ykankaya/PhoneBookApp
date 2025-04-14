using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ReportService.Domain.Entities;


namespace ReportService.Infrastructure.Persistence.Configurations
{
  public class ReportConfiguration : IEntityTypeConfiguration<Report>
  {
    public void Configure(EntityTypeBuilder<Report> builder)
    {
      builder.HasKey(r => r.Id);

      builder.Property(r => r.TalepEdildigiTarih)
          .IsRequired();

      builder.Property(r => r.Durum)
          .IsRequired()
          .HasConversion<string>()
          .HasMaxLength(50);

      
      builder.HasMany(r => r.RaporDetaylari)
            .WithOne(rd => rd.Report)
            .HasForeignKey(rd => rd.ReportId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
  }
}

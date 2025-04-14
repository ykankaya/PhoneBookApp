using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ReportService.Domain.Entities;


namespace ReportService.Infrastructure.Persistence.Configurations
{
  public class ReportDetailConfiguration : IEntityTypeConfiguration<ReportDetail>
  {
    public void Configure(EntityTypeBuilder<ReportDetail> builder)
    {
      builder.HasKey(rd => rd.Id);

      builder.Property(rd => rd.KonumBilgisi)
          .IsRequired()
          .HasMaxLength(150);

      builder.Property(rd => rd.KisiSayisi)
          .IsRequired();

      builder.Property(rd => rd.TelefonNumarasiSayisi)
          .IsRequired();

    }
  }
}

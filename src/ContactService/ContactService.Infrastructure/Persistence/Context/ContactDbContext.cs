using ContactService.Application.Interfaces.Persistence;
using ContactService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ContactService.Infrastructure.Persistence.Context
{
  public class ContactDbContext : DbContext, IContactDbContext
  {
    public ContactDbContext(DbContextOptions<ContactDbContext> options) : base(options)
    {
    }

    public DbSet<Person> Persons => Set<Person>();
    public DbSet<ContactInfo> ContactInfos => Set<ContactInfo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // Tüm IEntityTypeConfiguration arayüzünü implemente eden sınıfları
      // bu assembly içerisinden bulup uygular.
      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

      base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
      // İsteğe bağlı: Otomatik tarih ayarlama vb. eklenebilir.
      return base.SaveChangesAsync(cancellationToken);
    }
  }
}

using Microsoft.EntityFrameworkCore;
using ContactService.Domain.Entities;

namespace ContactService.Application.Interfaces.Persistence
{
  public interface IContactDbContext
  {
    DbSet<Person> Persons { get; }
    DbSet<ContactInfo> ContactInfos { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
  }
}

using Microsoft.EntityFrameworkCore;
using ContactService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ContactService.Application.Interfaces.Persistence
{
  public interface IContactDbContext
  {
    DbSet<Person> Persons { get; }
    DbSet<ContactInfo> ContactInfos { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
  }
}

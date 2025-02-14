using Athena.Domain.Entities;

namespace Athena.Domain.Repositories;

public interface IDataEntryRepository
{
    Task<DataEntry> GetByIdAsync(int id);
    Task<IEnumerable<DataEntry>> GetAllAsync();
    Task<DataEntry> AddAsync(DataEntry entry);
    Task UpdateAsync(DataEntry entry);
    Task<bool> ExistsAsync(int id);
}

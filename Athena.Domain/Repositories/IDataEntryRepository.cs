using Athena.Domain.Entities;

namespace Athena.Domain.Repositories;

public interface IDataEntryRepository
{
    Task<DataEntry> GetByIdAsync(Guid id);
    Task<(IEnumerable<DataEntry>, int)> GetAllAsync(string? category = null,
    string? tag = null,
    string? orderBy = null,
    int pageSize = 10,
    int pageNumber = 1,
    bool descending = false);
    Task<DataEntry> AddAsync(DataEntry entry);
    Task UpdateAsync(DataEntry entry);
    Task<bool> ExistsAsync(Guid id);
}

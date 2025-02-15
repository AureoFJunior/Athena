using Athena.Domain.Entities;
using Athena.Domain.Repositories;
using Athena.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Athena.Infrastructure.Repositories;

public class DataEntryRepository : IDataEntryRepository
{
    private readonly AppDbContext _context;

    public DataEntryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DataEntry> GetByIdAsync(Guid id)
    {
        return await _context.DataEntries.FindAsync(id) ?? new DataEntry("", "") { };
    }

    public async Task<(IEnumerable<DataEntry>, int)> GetAllAsync(
    string? category = null,
    string? tag = null,
    string? orderBy = null,
    int pageSize = 10,
    int pageNumber = 1,
    bool descending = false)
    {
        IQueryable<DataEntry> query = _context.DataEntries.AsNoTracking();

        int totalItems = await query.CountAsync();

        if (!string.IsNullOrEmpty(category)) // Filter by CATEGORY
            query = query.Where(d => d.Category == category);

        if (!string.IsNullOrEmpty(tag)) // Filter by TAG
            query = query.Where(d => d.Tags.Contains(tag));

        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        query = orderBy?.ToLower() switch
        {
            "id" => descending ? query.OrderByDescending(d => d.Id) : query.OrderBy(d => d.Id),
            "title" => descending ? query.OrderByDescending(d => d.Title) : query.OrderBy(d => d.Title),
            "description" => descending ? query.OrderByDescending(d => d.Description) : query.OrderBy(d => d.Description),
            "rating" => descending ? query.OrderByDescending(d => d.Rating) : query.OrderBy(d => d.Rating),
            "category" => descending ? query.OrderByDescending(d => d.Category) : query.OrderBy(d => d.Category),
            "createdat" => descending ? query.OrderByDescending(d => d.CreatedAt) : query.OrderBy(d => d.CreatedAt),
            "updatedat" => descending ? query.OrderByDescending(d => d.UpdatedAt) : query.OrderBy(d => d.UpdatedAt),
            _ => query // Without Order
        };

        return (await query.ToListAsync(), totalItems);
    }

    public async Task<DataEntry> AddAsync(DataEntry entry)
    {
        await _context.DataEntries.AddAsync(entry);
        await _context.SaveChangesAsync();
        entry = new DataEntry("", "");
        return entry;
    }

    public async Task UpdateAsync(DataEntry entry)
    {
        _context.Entry(entry).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.DataEntries.AnyAsync(e => e.Id == id);
    }
}
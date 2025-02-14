using Athena.Domain.Entities;
using Athena.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;

namespace Athena.Infrastructure.Repositories;

public class DataEntryRepository : IDataEntryRepository
{
    private readonly AppDbContext _context;

    public DataEntryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DataEntry> GetByIdAsync(int id)
    {
        return await _context.DataEntries.FindAsync(id);
    }

    public async Task<IEnumerable<DataEntry>> GetAllAsync()
    {
        return await _context.DataEntries.ToListAsync();
    }

    public async Task<DataEntry> AddAsync(DataEntry entry)
    {
        await _context.DataEntries.AddAsync(entry);
        await _context.SaveChangesAsync();
        return entry;
    }

    public async Task UpdateAsync(DataEntry entry)
    {
        _context.Entry(entry).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.DataEntries.AnyAsync(e => e.Id == id);
    }
}
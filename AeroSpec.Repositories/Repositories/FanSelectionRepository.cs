using Microsoft.EntityFrameworkCore;
using AeroSpec.Database;
using AeroSpec.Repositories.Contracts;

namespace AeroSpec.Repositories.Repositories;

public class FanSelectionRepository : IFanSelectionRepository
{
    private readonly ApplicationDbContext _context;

    public FanSelectionRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<FanSelection?> GetByIdAsync(int id)
    {
        return await _context.FanSelections
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
    }

    public async Task<IEnumerable<FanSelection>> GetAllActiveAsync()
    {
        return await _context.FanSelections
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<FanSelection>> GetByProjectNameAsync(string projectName)
    {
        return await _context.FanSelections
            .Where(x => x.ProjectName == projectName && x.IsActive)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<FanSelection>> GetByCreatedDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.FanSelections
            .Where(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.IsActive)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<FanSelection> AddAsync(FanSelection fanSelection)
    {
        if (fanSelection == null) throw new ArgumentNullException(nameof(fanSelection));

        fanSelection.CreatedDate = DateTime.UtcNow;
        fanSelection.ModifiedDate = DateTime.UtcNow;

        _context.FanSelections.Add(fanSelection);
        await _context.SaveChangesAsync();

        return fanSelection;
    }

    public async Task UpdateAsync(FanSelection fanSelection)
    {
        if (fanSelection == null) throw new ArgumentNullException(nameof(fanSelection));

        fanSelection.ModifiedDate = DateTime.UtcNow;
        _context.FanSelections.Update(fanSelection);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var fanSelection = await GetByIdAsync(id);
        if (fanSelection != null)
        {
            fanSelection.IsActive = false;
            fanSelection.ModifiedDate = DateTime.UtcNow;
            _context.FanSelections.Update(fanSelection);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.FanSelections
            .AnyAsync(x => x.Id == id && x.IsActive);
    }
}
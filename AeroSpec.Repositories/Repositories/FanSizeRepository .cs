using Microsoft.EntityFrameworkCore;
using AeroSpec.Database;

using AeroSpec.Repositories.Contracts;

namespace AeroSpec.Repositories.Repositories;

public class FanSizeRepository : IFanSizeRepository
{
    private readonly ApplicationDbContext _context;

    public FanSizeRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<FanSize?> GetByIdAsync(int id)
    {
        return await _context.FanSizes
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
    }

    public async Task<FanSize?> GetBySizeIdAsync(string sizeId)
    {
        return await _context.FanSizes
            .FirstOrDefaultAsync(x => x.SizeId == sizeId && x.IsActive);
    }

    public async Task<IEnumerable<FanSize>> GetAllActiveAsync()
    {
        return await _context.FanSizes
            .Where(x => x.IsActive)
            .OrderBy(x => x.SizeId)
            .ToListAsync();
    }

    public async Task<FanSize> AddAsync(FanSize fanSize)
    {
        if (fanSize == null) throw new ArgumentNullException(nameof(fanSize));

        fanSize.CreatedDate = DateTime.UtcNow;
        fanSize.ModifiedDate = DateTime.UtcNow;

        _context.FanSizes.Add(fanSize);
        await _context.SaveChangesAsync();

        return fanSize;
    }

    public async Task UpdateAsync(FanSize fanSize)
    {
        if (fanSize == null) throw new ArgumentNullException(nameof(fanSize));

        fanSize.ModifiedDate = DateTime.UtcNow;
        _context.FanSizes.Update(fanSize);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var fanSize = await GetByIdAsync(id);
        if (fanSize != null)
        {
            fanSize.IsActive = false;
            fanSize.ModifiedDate = DateTime.UtcNow;
            _context.FanSizes.Update(fanSize);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(string sizeId)
    {
        return await _context.FanSizes
            .AnyAsync(x => x.SizeId == sizeId && x.IsActive);
    }
}
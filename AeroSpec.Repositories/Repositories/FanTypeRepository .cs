using Microsoft.EntityFrameworkCore;
using AeroSpec.Database;
using AeroSpec.Repositories.Contracts;

namespace AeroSpec.Repositories.Repositories;

public class FanTypeRepository : IFanTypeRepository
{
    private readonly ApplicationDbContext _context;

    public FanTypeRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<FanType?> GetByIdAsync(int id)
    {
        return await _context.FanTypes
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
    }

    public async Task<FanType?> GetByTypeIdAsync(string typeId)
    {
        return await _context.FanTypes
            .FirstOrDefaultAsync(x => x.TypeId == typeId && x.IsActive);
    }

    public async Task<IEnumerable<FanType>> GetAllActiveAsync()
    {
        return await _context.FanTypes
            .Where(x => x.IsActive)
            .OrderBy(x => x.TypeId)
            .ToListAsync();
    }

    public async Task<FanType> AddAsync(FanType fanType)
    {
        if (fanType == null) throw new ArgumentNullException(nameof(fanType));

        fanType.CreatedDate = DateTime.UtcNow;
        fanType.ModifiedDate = DateTime.UtcNow;

        _context.FanTypes.Add(fanType);
        await _context.SaveChangesAsync();

        return fanType;
    }

    public async Task UpdateAsync(FanType fanType)
    {
        if (fanType == null) throw new ArgumentNullException(nameof(fanType));

        fanType.ModifiedDate = DateTime.UtcNow;
        _context.FanTypes.Update(fanType);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string typeId)
    {
        return await _context.FanTypes
            .AnyAsync(x => x.TypeId == typeId && x.IsActive);
    }
}
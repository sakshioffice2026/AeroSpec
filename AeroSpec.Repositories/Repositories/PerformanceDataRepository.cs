using Microsoft.EntityFrameworkCore;
using AeroSpec.Database;
using AeroSpec.Repositories.Contracts;

namespace AeroSpec.Repositories.Repositories;

public class PerformanceDataRepository : IPerformanceDataRepository
{
    private readonly ApplicationDbContext _context;

    public PerformanceDataRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<PerformanceData>> GetByFanSizeIdAsync(int fanSizeId)
    {
        return await _context.PerformanceData
            .Where(x => x.FanSizeId == fanSizeId && x.IsActive)
            .OrderBy(x => x.Rpm)
            .ToListAsync();
    }

    public async Task<IEnumerable<PerformanceData>> GetByFanSizeAndRpmAsync(int fanSizeId, int rpm)
    {
        return await _context.PerformanceData
            .Where(x => x.FanSizeId == fanSizeId && x.Rpm == rpm && x.IsActive)
            .ToListAsync();
    }

    public async Task<PerformanceData?> GetByFanSizeIdAndRpmAsync(int fanSizeId, int rpm)
    {
        return await _context.PerformanceData
            .FirstOrDefaultAsync(x => x.FanSizeId == fanSizeId && x.Rpm == rpm && x.IsActive);
    }

    public async Task AddAsync(PerformanceData data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        data.CreatedDate = DateTime.UtcNow;
        _context.PerformanceData.Add(data);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<PerformanceData> data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        var performanceDataList = data.ToList();
        foreach (var item in performanceDataList)
        {
            item.CreatedDate = DateTime.UtcNow;
        }

        _context.PerformanceData.AddRange(performanceDataList);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PerformanceData data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        _context.PerformanceData.Update(data);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByFanSizeAsync(int fanSizeId)
    {
        var dataToDelete = await _context.PerformanceData
            .Where(x => x.FanSizeId == fanSizeId && x.IsActive)
            .ToListAsync();

        foreach (var item in dataToDelete)
        {
            item.IsActive = false;
        }

        _context.PerformanceData.UpdateRange(dataToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int fanSizeId, int rpm)
    {
        return await _context.PerformanceData
            .AnyAsync(x => x.FanSizeId == fanSizeId && x.Rpm == rpm && x.IsActive);
    }
}
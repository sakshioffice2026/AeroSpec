using AeroSpec.Database;


namespace AeroSpec.Repositories.Contracts;

public interface IPerformanceDataRepository
{
    Task<IEnumerable<PerformanceData>> GetByFanSizeIdAsync(int fanSizeId);
    Task<IEnumerable<PerformanceData>> GetByFanSizeAndRpmAsync(int fanSizeId, int rpm);
    Task<PerformanceData?> GetByFanSizeIdAndRpmAsync(int fanSizeId, int rpm);
    Task AddAsync(PerformanceData data);
    Task AddRangeAsync(IEnumerable<PerformanceData> data);
    Task UpdateAsync(PerformanceData data);
    Task DeleteByFanSizeAsync(int fanSizeId);
    Task<bool> ExistsAsync(int fanSizeId, int rpm);
}


using AeroSpec.Database;

namespace AeroSpec.Repositories.Contracts;

public interface IFanSizeRepository
{
    Task<FanSize?> GetByIdAsync(int id);
    Task<FanSize?> GetBySizeIdAsync(string sizeId);
    Task<IEnumerable<FanSize>> GetAllActiveAsync();
    Task<FanSize> AddAsync(FanSize fanSize);
    Task UpdateAsync(FanSize fanSize);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(string sizeId);
}
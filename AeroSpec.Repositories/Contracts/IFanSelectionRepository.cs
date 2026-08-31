using AeroSpec.Database;

namespace AeroSpec.Repositories.Contracts;

public interface IFanSelectionRepository
{
    Task<FanSelection?> GetByIdAsync(int id);
    Task<IEnumerable<FanSelection>> GetAllActiveAsync();
    Task<IEnumerable<FanSelection>> GetByProjectNameAsync(string projectName);
    Task<IEnumerable<FanSelection>> GetByCreatedDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<FanSelection> AddAsync(FanSelection fanSelection);
    Task UpdateAsync(FanSelection fanSelection);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
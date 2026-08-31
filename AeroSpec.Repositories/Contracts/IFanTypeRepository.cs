using AeroSpec.Database;


namespace AeroSpec.Repositories.Contracts;

public interface IFanTypeRepository
{
    Task<FanType?> GetByIdAsync(int id);
    Task<FanType?> GetByTypeIdAsync(string typeId);
    Task<IEnumerable<FanType>> GetAllActiveAsync();
    Task<FanType> AddAsync(FanType fanType);
    Task UpdateAsync(FanType fanType);
    Task<bool> ExistsAsync(string typeId);
}
using AeroSpec.Database;
using AeroSpec.Models.DTOs;

namespace AeroSpec.Business.Contracts;

public interface IFanSelectionService
{
    Task<FanSelectionResultDto> ProcessSelectionAsync(SpecificationInputDto input);
    Task<FanSelection?> GetByIdAsync(int id);
    Task<IEnumerable<FanSelection>> GetHistoryAsync();
    Task<IEnumerable<FanSelection>> GetByProjectNameAsync(string projectName);
    Task DeleteAsync(int id);
}
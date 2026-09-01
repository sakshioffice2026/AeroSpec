using AeroSpec.Database;

namespace AeroSpec.Repositories.Contracts;

public interface IAccountRepository
{
    Task<AppUser?> GetByIdAsync(int id);
    Task<AppUser?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<AppUser> AddAsync(AppUser user);
    Task UpdateLastLoginAsync(AppUser user);
    Task UpdatePasswordAsync(AppUser user, string passwordHash);
}
using Microsoft.EntityFrameworkCore;
using AeroSpec.Database;
using AeroSpec.Repositories.Contracts;

namespace AeroSpec.Repositories.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _context;

    public AccountRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<AppUser?> GetByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
    }

    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email && x.IsActive);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(x => x.Email == email);
    }

    public async Task<AppUser> AddAsync(AppUser user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        user.CreatedDate = DateTime.UtcNow;
        user.ModifiedDate = DateTime.UtcNow;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task UpdateLastLoginAsync(AppUser user)
    {
        user.LastLoginDate = DateTime.UtcNow;
        user.ModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePasswordAsync(AppUser user, string passwordHash)
    {
        user.PasswordHash = passwordHash;
        user.ModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
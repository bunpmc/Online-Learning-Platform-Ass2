using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatformAss2.Data.Entities;
using OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

namespace OnlineLearningPlatformAss2.Data.Repositories;

public class UserRepository(OnlineLearningSystemDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserWithRoleAsync(Guid userId)
    {
        return await context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User?> GetUserWithRoleAndProfileAsync(string usernameOrEmail)
    {
        return await context.Users
            .Include(u => u.Role)
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u =>
                u.Username.ToLower() == usernameOrEmail.ToLower() ||
                u.Email.ToLower() == usernameOrEmail.ToLower());
    }

    public async Task<User?> GetUserWithRoleAndProfileByIdAsync(Guid userId)
    {
        return await context.Users
            .Include(u => u.Role)
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<IEnumerable<User>> GetAllWithRolesAsync()
    {
        return await context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> SearchUsersAsync(string? searchTerm)
    {
        var query = context.Users.AsNoTracking().Include(u => u.Role).AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(u => u.Username.ToLower().Contains(term) || u.Email.ToLower().Contains(term));
        }

        return await query.OrderByDescending(u => u.CreatedAt).ToListAsync();
    }

    public async Task<bool> ExistsAsync(string usernameOrEmail)
    {
        return await context.Users.AnyAsync(u =>
            u.Username.ToLower() == usernameOrEmail.ToLower() ||
            u.Email.ToLower() == usernameOrEmail.ToLower());
    }

    public async Task<Role?> GetRoleByNameAsync(string roleName)
    {
        return await context.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == roleName.ToLower());
    }

    public async Task<Profile?> GetProfileByUserIdAsync(Guid userId)
    {
        return await context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task AddAsync(User user)
    {
        await context.Users.AddAsync(user);
    }

    public async Task AddProfileAsync(Profile profile)
    {
        await context.Profiles.AddAsync(profile);
    }

    public Task UpdateAsync(User user)
    {
        context.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task UpdateProfileAsync(Profile profile)
    {
        context.Profiles.Update(profile);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(User user)
    {
        context.Users.Remove(user);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();
}

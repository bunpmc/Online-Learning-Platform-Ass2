using OnlineLearningPlatformAss2.Data.Entities;

namespace OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetUserWithRoleAsync(Guid userId);
    Task<User?> GetUserWithRoleAndProfileAsync(string usernameOrEmail);
    Task<User?> GetUserWithRoleAndProfileByIdAsync(Guid userId);
    Task<IEnumerable<User>> GetAllWithRolesAsync();
    Task<IEnumerable<User>> SearchUsersAsync(string? searchTerm);
    Task<bool> ExistsAsync(string usernameOrEmail);
    Task<Role?> GetRoleByNameAsync(string roleName);
    Task<Profile?> GetProfileByUserIdAsync(Guid userId);
    Task AddAsync(User user);
    Task AddProfileAsync(Profile profile);
    Task UpdateAsync(User user);
    Task UpdateProfileAsync(Profile profile);
    Task RemoveAsync(User user);
    Task<int> SaveChangesAsync();
}

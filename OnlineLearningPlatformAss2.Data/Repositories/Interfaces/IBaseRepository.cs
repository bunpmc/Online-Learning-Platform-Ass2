using OnlineLearningPlatformAss2.Data.Database.Entities;

namespace OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

public interface IBaseRepository<T> where T : BaseEntity
{
    // Read Operations
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllWithDeletedAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    // Create Operations
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    // Update Operations
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    // Delete Operations
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<bool> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);

    // Save Changes
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

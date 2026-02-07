using OnlineLearningPlatformAss2.Data.Entities;

namespace OnlineLearningPlatformAss2.Data.Repositories.Interfaces;

public interface ICourseRepository
{
    // Course queries
    Task<Course?> GetByIdAsync(Guid id);
    Task<Course?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Course>> GetFeaturedCoursesAsync(int limit);
    Task<IEnumerable<Course>> GetCoursesAsync(string? searchTerm = null, Guid? categoryId = null, int? limit = null);
    Task<IEnumerable<Course>> GetInstructorCoursesAsync(Guid instructorId);
    Task<int> GetEnrollmentCountAsync(Guid courseId);
    
    // Course mutations
    Task UpdateAsync(Course course);
    Task<int> SaveChangesAsync();
    
    // Wishlist
    Task<Wishlist?> GetWishlistItemAsync(Guid userId, Guid courseId);
    Task<IEnumerable<Wishlist>> GetUserWishlistAsync(Guid userId);
    Task AddWishlistAsync(Wishlist wishlist);
    Task RemoveWishlistAsync(Wishlist wishlist);
    Task<bool> IsInWishlistAsync(Guid userId, Guid courseId);
    
    // Category
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    
    // Module
    Task<Module?> GetModuleByIdAsync(Guid moduleId);
    Task AddModuleAsync(Module module);
    Task UpdateModuleAsync(Module module);
    Task RemoveModuleAsync(Module module);
    
    // Lesson
    Task<Lesson?> GetLessonByIdAsync(Guid lessonId);
    Task AddLessonAsync(Lesson lesson);
    Task UpdateLessonAsync(Lesson lesson);
    Task RemoveLessonAsync(Lesson lesson);
    
    // Orders/Earnings
    Task<decimal> GetInstructorEarningsAsync(Guid instructorId);
    
    // Certificate
    Task<Certificate?> GetCertificateByIdAsync(Guid certificateId);
    Task<bool> CertificateExistsAsync(Guid enrollmentId);
    Task AddCertificateAsync(Certificate certificate);
}

using OnlineLearningPlatformAss2.Service.Services.Interfaces;
using OnlineLearningPlatformAss2.Service.DTOs.User;
using OnlineLearningPlatformAss2.Service.Results;
using OnlineLearningPlatformAss2.Data.Entities;
using OnlineLearningPlatformAss2.Service.DTOs.Course;
using OnlineLearningPlatformAss2.Data.Repositories.Interfaces;
using BCrypt.Net;

namespace OnlineLearningPlatformAss2.Service.Services;

public class UserService(
    IUserRepository userRepository,
    IEnrollmentRepository enrollmentRepository) : IUserService
{
    public async Task<ServiceResult<Guid>> RegisterAsync(UserRegisterDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                return ServiceResult<Guid>.FailureResult("Username is required.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                return ServiceResult<Guid>.FailureResult("Email is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return ServiceResult<Guid>.FailureResult("Password is required.");

            if (dto.Password != dto.ConfirmPassword)
                return ServiceResult<Guid>.FailureResult("Passwords do not match.");

            if (dto.Password.Length < 6)
                return ServiceResult<Guid>.FailureResult("Password must be at least 6 characters long.");

            var existingUserByUsername = await userRepository.GetByUsernameAsync(dto.Username);
            if (existingUserByUsername != null)
                return ServiceResult<Guid>.FailureResult("Username already exists.");

            var existingUserByEmail = await userRepository.GetByEmailAsync(dto.Email);
            if (existingUserByEmail != null)
                return ServiceResult<Guid>.FailureResult("Email already exists.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var defaultRole = await userRepository.GetRoleByNameAsync("user");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username.Trim(),
                Email = dto.Email.Trim().ToLower(),
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                RoleId = defaultRole?.Id,
                HasCompletedAssessment = false
            };

            await userRepository.AddAsync(user);
            await userRepository.SaveChangesAsync();

            return ServiceResult<Guid>.SuccessResult(user.Id);
        }
        catch (Exception ex)
        {
            return ServiceResult<Guid>.FailureResult($"Registration failed: {ex.Message}");
        }
    }

    public async Task<ServiceResult<UserLoginResponseDto>> LoginAsync(UserLoginDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.UsernameOrEmail))
                return ServiceResult<UserLoginResponseDto>.FailureResult("Username or email is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return ServiceResult<UserLoginResponseDto>.FailureResult("Password is required.");

            var usernameOrEmail = string.IsNullOrWhiteSpace(dto.UsernameOrEmail) ? dto.Email : dto.UsernameOrEmail;
            var user = await userRepository.GetUserWithRoleAndProfileAsync(usernameOrEmail);

            if (user == null)
                return ServiceResult<UserLoginResponseDto>.FailureResult("Invalid username/email or password.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return ServiceResult<UserLoginResponseDto>.FailureResult("Invalid username/email or password.");

            var response = new UserLoginResponseDto
            {
                Id = user.Id,
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role?.Name,
                RoleName = user.Role?.Name ?? "User",
                FirstName = user.Profile?.FirstName ?? string.Empty,
                LastName = user.Profile?.LastName ?? string.Empty,
                AvatarUrl = user.Profile?.AvatarUrl,
                CreatedAt = user.CreatedAt
            };

            return ServiceResult<UserLoginResponseDto>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            return ServiceResult<UserLoginResponseDto>.FailureResult($"Login failed: {ex.Message}");
        }
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        try
        {
            var users = await userRepository.GetAllWithRolesAsync();
            return users.Select(u => new UserDto(
                u.Id,
                u.Username,
                u.Email,
                u.Role?.Name,
                u.CreatedAt
            ));
        }
        catch
        {
            return Enumerable.Empty<UserDto>();
        }
    }

    public async Task<bool> HasCompletedAssessmentAsync(Guid userId)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(userId);
            return user?.HasCompletedAssessment ?? false;
        }
        catch
        {
            return false;
        }
    }

    public async Task UpdateAssessmentStatusAsync(Guid userId, bool completed)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.HasCompletedAssessment = completed;
                if (completed)
                {
                    user.AssessmentCompletedAt = DateTime.UtcNow;
                }

                await userRepository.UpdateAsync(user);
                await userRepository.SaveChangesAsync();
            }
        }
        catch
        {
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        try
        {
            var user = await userRepository.GetUserWithRoleAsync(userId);
            if (user == null)
                return null;

            return new UserDto(
                user.Id,
                user.Username,
                user.Email,
                user.Role?.Name,
                user.CreatedAt
            );
        }
        catch
        {
            return null;
        }
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(Guid userId)
    {
        try
        {
            var userEntity = await userRepository.GetUserWithRoleAndProfileByIdAsync(userId);
            if (userEntity == null) return null;

            var enrolledCourses = await enrollmentRepository.GetStudentEnrollmentsDetailedAsync(userId);

            var recentCourses = enrolledCourses.Take(3).Select(e => new CourseViewModel
            {
                Id = e.Course.CourseId,
                Title = e.Course.Title,
                Description = e.Course.Description ?? string.Empty,
                Price = e.Course.Price,
                ImageUrl = e.Course.ImageUrl,
                CategoryName = e.Course.Category.Name,
                InstructorName = e.Course.Instructor.Username,
                Rating = 0m,
                EnrollmentDate = e.EnrolledAt,
                Progress = 0
            }).ToList();

            return new UserProfileDto(
                userEntity.Id,
                userEntity.Username,
                userEntity.Email,
                userEntity.Role?.Name,
                userEntity.CreatedAt,
                userEntity.Profile?.FirstName,
                userEntity.Profile?.LastName,
                userEntity.Profile?.AvatarUrl,
                enrolledCourses.Count(),
                enrolledCourses.Count(e => e.Status == "Completed"),
                enrolledCourses.Any() ? 0 : 0,
                userEntity.HasCompletedAssessment,
                recentCourses
            );
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UserExistsAsync(string usernameOrEmail)
    {
        try
        {
            return await userRepository.ExistsAsync(usernameOrEmail);
        }
        catch
        {
            return false;
        }
    }

    public async Task UpdateProfileAsync(Guid userId, dynamic updateRequest)
    {
        var profile = await userRepository.GetProfileByUserIdAsync(userId);

        if (profile == null)
        {
            profile = new Profile { Id = Guid.NewGuid(), UserId = userId };
            await userRepository.AddProfileAsync(profile);
        }
        
        try {
            profile.FirstName = (string)updateRequest.FirstName;
            profile.LastName = (string)updateRequest.LastName;
            profile.AvatarUrl = (string)updateRequest.AvatarUrl;
        } catch {}

        await userRepository.UpdateProfileAsync(profile);
        await userRepository.SaveChangesAsync();
    }

    public async Task<bool> UpgradeToInstructorAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        var instructorRole = await userRepository.GetRoleByNameAsync("instructor");
        if (instructorRole == null) return false;

        user.RoleId = instructorRole.Id;
        await userRepository.UpdateAsync(user);
        await userRepository.SaveChangesAsync();
        return true;
    }
}

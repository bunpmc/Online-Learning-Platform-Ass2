# Online Learning Platform

A modern, scalable online learning platform built with ASP.NET Core and Entity Framework Core. This project provides a comprehensive solution for managing courses, instructors, and student learning experiences.

## 📋 Table of Contents

- [Project Overview](#project-overview)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Database Setup](#database-setup)
- [Development Guide](#development-guide)
- [API Documentation](#api-documentation)
- [Contributing](#contributing)

## 🎯 Project Overview

The Online Learning Platform is a comprehensive educational system designed to facilitate:

- **Course Management**: Create, manage, and organize courses
- **User Management**: Manage students, instructors, and administrators
- **Role-Based Access Control**: Different permission levels for various user types
- **Profile Management**: User profiles with personal information and avatars

## 🏗️ Architecture

The project follows a **3-tier layered architecture**:

```
OnlineLearningPlatformAss2.RazorWebApp (Presentation Layer)
    ↓
OnlineLearningPlatformAss2.Service (Business Logic Layer)
    ↓
OnlineLearningPlatformAss2.Data (Data Access Layer)
```

### Layers

| Layer | Purpose | Responsibilities |
|-------|---------|------------------|
| **RazorWebApp** | Presentation | UI rendering, user interactions, HTTP requests/responses |
| **Service** | Business Logic | Business rules, data validation, service orchestration |
| **Data** | Data Access | Database operations, Entity Framework Core, repositories |

## 🛠️ Tech Stack

### Core Framework

- **.NET 9.0**: Latest LTS version of .NET
- **ASP.NET Core**: Web framework
- **Razor Pages**: Server-side rendering

### Data & ORM

- **Entity Framework Core 9.0**: ORM for database operations
- **SQL Server**: Primary database (can be configured)

### Development Tools

- **Rider / Visual Studio Code**: IDE
- **dotnet-ef**: Entity Framework Core CLI tools

### Dependencies (Managed in Directory.Packages.props)

- FluentValidation
- AutoMapper
- Serilog (Logging)
- xUnit (Testing)
- Moq (Mocking)

## 📦 Prerequisites

### Required

- **.NET SDK 9.0** or later
  - Download: <https://dotnet.microsoft.com/en-us/download/dotnet/9.0>
- **Git**: For version control
- **SQL Server** (2019 or later) or SQL Server Express

### Optional

- **Rider or Visual Studio**: IDE
- **VS Code with C# DevKit**
- **Entity Framework Core Tools**: For database migrations

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/PRN222-Group-1/Online-Learning-Platform-Ass2.git
cd OnlineLearningPlatformAss2
```

### 2. Restore NuGet Packages

```bash
dotnet restore
```

### 3. Configure Database Connection

Update the connection string in `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=OnlineLearningPlatformDb;Trusted_Connection=true;Encrypt=false;"
  }
}
```

**Connection String Examples:**

- **Local SQL Server**: `Server=(local);Database=OnlineLearningPlatformDb;Trusted_Connection=true;Encrypt=false;`
- **SQL Server Express**: `Server=.\SQLEXPRESS;Database=OnlineLearningPlatformDb;Trusted_Connection=true;Encrypt=false;`
- **Azure SQL**: `Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=yourdb;Persist Security Info=False;User ID=yourusername;Password=yourpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;`

### 4. Create DbContext

Create a file `OnlineLearningPlatformDbContext.cs` in the `Data` project:

```csharp
using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatformAss2.Data.Database.Entities;
using OnlineLearningPlatformAss2.Data.Database.EntityConfigurations;

namespace OnlineLearningPlatformAss2.Data.Database;

public class OnlineLearningPlatformDbContext : DbContext
{
    public OnlineLearningPlatformDbContext(DbContextOptions<OnlineLearningPlatformDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Profile> Profiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileConfiguration());
    }
}
```

### 5. Apply Migrations

```bash
# Create initial migration
dotnet ef migrations add InitialCreate --project OnlineLearningPlatformAss2.Data --startup-project OnlineLearningPlatformAss2.RazorWebApp

# Apply migrations to database
dotnet ef database update --project OnlineLearningPlatformAss2.Data --startup-project OnlineLearningPlatformAss2.RazorWebApp
```

### 6. Run the Application

```bash
dotnet run --project OnlineLearningPlatformAss2.RazorWebApp
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`.

## 📁 Project Structure

```
OnlineLearningPlatformAss2/
├── OnlineLearningPlatformAss2.RazorWebApp/     # Web UI Layer
│   ├── Pages/                                   # Razor pages
│   ├── wwwroot/                                # Static files (CSS, JS)
│   ├── appsettings.json                       # Configuration
│   └── Program.cs                              # Application startup
│
├── OnlineLearningPlatformAss2.Service/         # Business Logic Layer
│   ├── Services/                               # Service implementations
│   └── Interfaces/                             # Service contracts
│
├── OnlineLearningPlatformAss2.Data/            # Data Access Layer
│   ├── Database/
│   │   ├── Entities/                          # Domain models
│   │   │   ├── BaseEntity.cs
│   │   │   ├── User.cs
│   │   │   ├── Role.cs
│   │   │   └── Profile.cs
│   │   └── EntityConfigurations/              # EF Core configurations
│   │       ├── UserConfiguration.cs
│   │       ├── RoleConfiguration.cs
│   │       └── ProfileConfiguration.cs
│   └── Repositories/
│       ├── Interfaces/                        # Repository contracts
│       └── Implementations/                   # Repository implementations
│
├── Directory.Build.props                       # Shared build properties
├── Directory.Packages.props                    # Centralized NuGet versions
└── global.json                                 # Global SDK version
```

## 🗄️ Database Setup

### Entity Relationship Diagram

```
┌─────────────┐
│    User     │
├─────────────┤
│ Id (PK)     │
│ Email       │◄──────────┐
│ PasswordHash│           │
│ RoleId (FK) │           │
│ CreatedAt   │           │
│ UpdatedAt   │           │
│ IsDeleted   │           │
└─────────────┘           │
      ▲                   │
      │                   │
      │ 1:1               │ 1:1
      │                   │
      └───────────────────┤
                          │
                    ┌──────────────┐
                    │   Profile    │
                    ├──────────────┤
                    │ Id (PK)      │
                    │ UserId (FK)  │
                    │ FirstName    │
                    │ LastName     │
                    │ AvatarUrl    │
                    │ Description  │
                    └──────────────┘

┌─────────────┐
│    Role     │
├─────────────┤
│ Id (PK)     │
│ Name        │◄──────────────────┐
│ Description │                   │
│ CreatedAt   │                   │
│ UpdatedAt   │      1:N          │
│ IsDeleted   │                   │
└─────────────┘                   │
                           ┌──────────┐
                           │   User   │
                           └──────────┘
```

### Seeded Data

Default roles are automatically seeded on database creation:

- **Admin**: Full system access
- **Instructor**: Can create and manage courses
- **Student**: Can enroll and view courses

## 👨‍💻 Development Guide

### 1. Adding a New Entity

1. Create the entity in `OnlineLearningPlatformAss2.Data/Database/Entities/`
2. Inherit from `BaseEntity` to get automatic ID, timestamps, and soft delete support:

```csharp
public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid InstructorId { get; set; }
    public User Instructor { get; set; } = null!;
}
```

1. Create configuration in `EntityConfigurations/`:

```csharp
public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Title).IsRequired().HasMaxLength(200);
        // ... more configuration
    }
}
```

### 2. Creating a Repository

Repositories automatically inherit from `BaseRepository<T>`:

```csharp
public interface ICourseRepository : IBaseRepository<Course>
{
    Task<IEnumerable<Course>> GetCoursesByInstructorAsync(Guid instructorId);
    Task<IEnumerable<Course>> SearchCoursesAsync(string keyword);
}

public class CourseRepository : BaseRepository<Course>, ICourseRepository
{
    public CourseRepository(DbContext context) : base(context) { }

    // Implement custom methods
}
```

### 3. Creating a Service

Services contain business logic and should be registered in `Program.cs`:

```csharp
public interface ICourseService
{
    Task<CourseDto> CreateCourseAsync(CreateCourseDto dto);
    Task<CourseDto> GetCourseByIdAsync(Guid id);
}

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    // Implement methods
}
```

### 4. Dependency Injection Setup

In `Program.cs`:

```csharp
// Register repositories
builder.Services.AddScoped<ICourseRepository, CourseRepository>();

// Register services
builder.Services.AddScoped<ICourseService, CourseService>();
```

### 5. Soft Delete vs Hard Delete

- **Soft Delete** (Default): Sets `IsDeleted = true`, data remains in database
  - Use: `await courseRepository.DeleteAsync(id);`
  - Query automatically excludes soft-deleted items

- **Hard Delete** (Permanent): Permanently removes from database
  - Use: `await courseRepository.HardDeleteAsync(id);`
  - Use cautiously!

## 📚 API Documentation

### Common Repository Operations

```csharp
// Get by ID (excludes soft-deleted)
var course = await courseRepository.GetByIdAsync(courseId);

// Get all (excludes soft-deleted)
var courses = await courseRepository.GetAllAsync();

// Get all including soft-deleted
var allCourses = await courseRepository.GetAllWithDeletedAsync();

// Check if exists
bool exists = await courseRepository.ExistsAsync(courseId);

// Create
var newCourse = await courseRepository.AddAsync(new Course { ... });

// Batch create
var courses = await courseRepository.AddRangeAsync(new[] { ... });

// Update
course.Title = "Updated Title";
await courseRepository.UpdateAsync(course);

// Soft delete
await courseRepository.DeleteAsync(courseId);

// Hard delete
await courseRepository.HardDeleteAsync(courseId);

// Save (if making multiple changes)
await courseRepository.SaveChangesAsync();
```

## 🤝 Contributing

### Code Style Guidelines

1. **Naming Conventions**:
   - Classes/Interfaces: PascalCase (e.g., `CourseService`)
   - Methods/Properties: PascalCase
   - Private fields: camelCase with underscore (e.g., `_courseRepository`)
   - Local variables: camelCase

2. **Null Safety**:
   - Enable nullable reference types: `<Nullable>enable</Nullable>` in .csproj
   - Use `null!` only when absolutely certain

3. **Async/Await**:
   - Always use async/await for I/O operations
   - Method names should end with `Async`
   - Always accept `CancellationToken`

### Pull Request Process

1. Create a feature branch: `git checkout -b feature/your-feature-name`
2. Make your changes following code guidelines
3. Test locally: `dotnet test`
4. Commit with descriptive messages: `git commit -m "feat: add course enrollment feature"`
5. Push to remote: `git push origin feature/your-feature-name`
6. Create a pull request with description of changes

### Commit Message Format

```
<type>: <subject>

<body>

<footer>
```

**Types**: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`

**Examples**:

- `feat: add course search functionality`
- `fix: fix user registration email validation`
- `docs: update database setup instructions`

## ❓ Troubleshooting

### Database Connection Issues

```bash
# Check connection string in appsettings.json
# Ensure SQL Server is running
# Test connection: dotnet ef database update --dry-run
```

### Migration Issues

```bash
# Remove last migration if not applied
dotnet ef migrations remove

# View pending migrations
dotnet ef migrations list
```

### Port Already in Use

```bash
# Change port in Properties/launchSettings.json
# Or use: dotnet run --urls "https://localhost:5555"
```

## 📞 Support

For issues or questions:

1. Check existing GitHub issues
2. Create a new issue with detailed description
3. Contact the development team

## 📄 License

This project is proprietary and for educational purposes only.

---

**Last Updated**: January 2026
**Version**: 1.0.0

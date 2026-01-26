using OnlineLearningPlatformAss2.Data.Database.Entities;

namespace OnlineLearningPlatformAss2.Data.Database;

public class OnlineLearningPlatformAss2Context(DbContextOptions<OnlineLearningPlatformAss2Context> options) : DbContext(options)
{
  public DbSet<User> Users { get; set; } = null!;
  public DbSet<Profile> Profiles { get; set; } = null!;
  public DbSet<Role> Roles { get; set; } = null!;
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(OnlineLearningPlatformAss2Context).Assembly);
  }
}
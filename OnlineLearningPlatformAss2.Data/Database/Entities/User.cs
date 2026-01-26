namespace OnlineLearningPlatformAss2.Data.Database.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    
    public Guid? RoleId { get; set; }
    public Role? Role { get; set; }

    public Profile? Profile { get; set; }

}
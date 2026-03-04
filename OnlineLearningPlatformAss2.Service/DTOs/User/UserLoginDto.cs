namespace OnlineLearningPlatformAss2.Service.DTOs.User;

public class UserLoginDto
{
    public string UsernameOrEmail { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}

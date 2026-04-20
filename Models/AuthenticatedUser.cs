namespace desktop_app.Models;

public class AuthenticatedUser
{
    public string UserId { get; set; } = "";
    public string Username { get; set; } = "";
    public UserRole Role { get; set; }
}
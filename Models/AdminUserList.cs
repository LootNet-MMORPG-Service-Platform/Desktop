using System;

namespace desktop_app.Models;

public class AdminUserList
{
    public Guid Id { get; set; }
    public string Username { get; set; } = "";
    public UserRole Role { get; set; }
    public decimal Currency { get; set; }
    public bool IsBlocked { get; set; }
}

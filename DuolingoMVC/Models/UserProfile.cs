namespace DuolingoMVC.Models;

public class UserProfile
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int StreakDays { get; set; }
    public int Points { get; set; }
    public int Level { get; set; }
    public int XP { get; set; }
    public string NativeLanguage { get; set; } = string.Empty;
    public string TargetLanguage { get; set; } = string.Empty;
}

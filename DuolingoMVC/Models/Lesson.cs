namespace DuolingoMVC.Models;

public class Lesson
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int XPReward { get; set; }
    public int ProgressPercent { get; set; }
}

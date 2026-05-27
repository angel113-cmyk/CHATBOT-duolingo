namespace DuolingoMVC.Models;

public class HomeDashboardViewModel
{
    public UserProfile User { get; set; } = new();
    public List<Lesson> Lessons { get; set; } = new();

    public int CompletedLessons => Lessons.Count(lesson => lesson.Status == "Completado");
    public int TotalLessons => Lessons.Count;
    public int CompletionPercent => TotalLessons == 0 ? 0 : (int)Math.Round(CompletedLessons * 100.0 / TotalLessons);
}

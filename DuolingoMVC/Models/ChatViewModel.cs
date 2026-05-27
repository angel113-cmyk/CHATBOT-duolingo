namespace DuolingoMVC.Models;

public class ChatViewModel
{
    public List<ChatMessage> Messages { get; set; } = new();
    public string CurrentLevel { get; set; } = string.Empty;
    public List<string> LevelOptions { get; set; } = new() { "A1", "A2", "B1" };
    public string UserInput { get; set; } = string.Empty;
}

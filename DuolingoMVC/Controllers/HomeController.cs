using System.Diagnostics;
using System.Text.Json;
using DuolingoMVC.Data;
using DuolingoMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DuolingoMVC.Controllers;

public class HomeController : Controller
{
    private const string SessionKeyUserId = "UserProfileId";
    private const string SessionKeyChatLevel = "ChatLevel";
    private const string SessionKeyChatMessages = "ChatMessages";
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _db;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    private UserProfile? GetCurrentUser()
    {
        var userId = HttpContext.Session.GetInt32(SessionKeyUserId);
        return userId.HasValue ? _db.UserProfiles.Find(userId.Value) : null;
    }

    private string GetChatLevel() => HttpContext.Session.GetString(SessionKeyChatLevel) ?? string.Empty;

    private List<ChatMessage> GetChatMessages()
    {
        var data = HttpContext.Session.GetString(SessionKeyChatMessages);
        return string.IsNullOrEmpty(data)
            ? new List<ChatMessage>()
            : JsonSerializer.Deserialize<List<ChatMessage>>(data) ?? new List<ChatMessage>();
    }

    private void SaveChatMessages(List<ChatMessage> messages)
    {
        HttpContext.Session.SetString(SessionKeyChatMessages, JsonSerializer.Serialize(messages));
    }

    private void SaveChatLevel(string level)
    {
        HttpContext.Session.SetString(SessionKeyChatLevel, level);
    }

    private string GenerateBotResponse(string level, string message)
    {
        var lower = message?.Trim().ToLowerInvariant() ?? string.Empty;

        if (level == "A1")
        {
            if (lower.Contains("hola") || lower.Contains("buenos"))
                return "¡Hola! Estoy bien. ¿Y tú cómo estás?";
            if (lower.Contains("cómo") || lower.Contains("que tal"))
                return "Muy bien, gracias. ¿Quieres practicar más palabras fáciles?";
            if (lower.Contains("gracias"))
                return "De nada. ¡Sigamos practicando!";
            return "Vamos a hablar con palabras simples. ¿Sobre qué quieres hablar?";
        }

        if (level == "A2")
        {
            if (lower.Contains("hola") || lower.Contains("buenos"))
                return "Hola, ¿qué tal? Podemos hablar de tu día o de tus pasatiempos.";
            if (lower.Contains("qué"))
                return "Es una buena pregunta. Cuéntame más sobre tus intereses.";
            if (lower.Contains("gracias"))
                return "De nada. Estoy aquí para ayudarte con tu idioma.";
            return "Hablemos usando frases cortas y claras. ¿Qué te gusta hacer?";
        }

        if (level == "B1")
        {
            if (lower.Contains("hola") || lower.Contains("buenos"))
                return "Hola, vamos a conversar con un poco más de complejidad. Puedes hablar sobre tu trabajo o tus metas.";
            if (lower.Contains("qué"))
                return "Esa es una pregunta interesante. ¿Puedes explicar por qué te interesa ese tema?";
            if (lower.Contains("gracias"))
                return "De nada. Me alegra ayudar en tu aprendizaje.";
            return "Puedo usar expresiones más completas. ¿Te gustaría practicar una conversación real?";
        }

        return "Elige primero un nivel: A1, A2 o B1. Puedes seleccionar uno en la parte superior o escribir /nivel A1.";
    }

    public IActionResult Index()
    {
        var user = GetCurrentUser();
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var lessons = _db.Lessons.OrderBy(l => l.Id).ToList();
        return View(new HomeDashboardViewModel { User = user, Lessons = lessons });
    }

    public IActionResult Lessons()
    {
        var user = GetCurrentUser();
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var lessons = _db.Lessons.OrderBy(l => l.Id).ToList();
        return View(new HomeDashboardViewModel { User = user, Lessons = lessons });
    }

    public IActionResult Profile()
    {
        var user = GetCurrentUser();
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        return View(user);
    }

    public IActionResult Chat()
    {
        var user = GetCurrentUser();
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var level = GetChatLevel();
        var messages = GetChatMessages();

        if (!messages.Any())
        {
            messages.Add(new ChatMessage
            {
                Sender = "Bot",
                Text = "¡Bienvenido al chat de idioma! Por favor elige tu nivel: A1, A2 o B1."
            });
            SaveChatMessages(messages);
        }

        return View(new ChatViewModel
        {
            Messages = messages,
            CurrentLevel = level,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Chat(string userInput)
    {
        var user = GetCurrentUser();
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        if (string.IsNullOrWhiteSpace(userInput))
        {
            return RedirectToAction("Chat");
        }

        var level = GetChatLevel();
        var messages = GetChatMessages();
        messages.Add(new ChatMessage { Sender = "User", Text = userInput.Trim() });

        if (userInput.StartsWith("/nivel", StringComparison.OrdinalIgnoreCase))
        {
            var requested = userInput.Replace("/nivel", string.Empty, StringComparison.OrdinalIgnoreCase).Trim().ToUpperInvariant();
            if (requested == "A1" || requested == "A2" || requested == "B1")
            {
                SaveChatLevel(requested);
                messages.Add(new ChatMessage { Sender = "Bot", Text = $"Has cambiado a nivel {requested}." });
                SaveChatMessages(messages);
                return RedirectToAction("Chat");
            }

            messages.Add(new ChatMessage { Sender = "Bot", Text = "Nivel no válido. Escribe /nivel A1, /nivel A2 o /nivel B1." });
            SaveChatMessages(messages);
            return RedirectToAction("Chat");
        }

        if (string.IsNullOrEmpty(level))
        {
            messages.Add(new ChatMessage { Sender = "Bot", Text = "Primero debes elegir un nivel: A1, A2 o B1." });
            SaveChatMessages(messages);
            return RedirectToAction("Chat");
        }

        var botResponse = GenerateBotResponse(level, userInput);
        messages.Add(new ChatMessage { Sender = "Bot", Text = botResponse });
        SaveChatMessages(messages);
        return RedirectToAction("Chat");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetLevel(string level)
    {
        var user = GetCurrentUser();
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var normalized = level?.Trim().ToUpperInvariant();
        if (normalized != "A1" && normalized != "A2" && normalized != "B1")
        {
            return RedirectToAction("Chat");
        }

        SaveChatLevel(normalized);
        var messages = GetChatMessages();
        messages.Add(new ChatMessage { Sender = "Bot", Text = $"Has elegido nivel {normalized}. Ya puedo ajustar mis respuestas." });
        SaveChatMessages(messages);
        return RedirectToAction("Chat");
    }

    public IActionResult Login()
    {
        if (GetCurrentUser() != null)
        {
            return RedirectToAction("Index");
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = _db.UserProfiles.SingleOrDefault(u => u.Email == model.Email && u.Password == model.Password);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrecta.");
            return View(model);
        }

        HttpContext.Session.SetInt32(SessionKeyUserId, user.Id);
        return RedirectToAction("Index");
    }

    public IActionResult Register()
    {
        if (GetCurrentUser() != null)
        {
            return RedirectToAction("Index");
        }

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (_db.UserProfiles.Any(u => u.Email == model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "Este correo ya está registrado.");
            return View(model);
        }

        var user = new UserProfile
        {
            Name = model.Name,
            Email = model.Email,
            Password = model.Password,
            StreakDays = 1,
            Points = 0,
            Level = 1,
            XP = 0,
            NativeLanguage = "Español",
            TargetLanguage = "Inglés"
        };

        _db.UserProfiles.Add(user);
        _db.SaveChanges();

        HttpContext.Session.SetInt32(SessionKeyUserId, user.Id);
        return RedirectToAction("Index");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

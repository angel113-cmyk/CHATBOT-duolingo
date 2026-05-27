using System.Diagnostics;
using DuolingoMVC.Data;
using DuolingoMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DuolingoMVC.Controllers;

public class HomeController : Controller
{
    private const string SessionKeyUserId = "UserProfileId";
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
        HttpContext.Session.Remove(SessionKeyUserId);
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

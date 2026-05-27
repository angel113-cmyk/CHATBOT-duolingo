using DuolingoMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace DuolingoMVC.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Lesson> Lessons => Set<Lesson>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserProfile>().HasData(
            new UserProfile
            {
                Id = 1,
                Name = "Ari",
                Email = "ari@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Demo123!"),
                StreakDays = 7,
                Points = 1250,
                Level = 4,
                XP = 3200,
                NativeLanguage = "Español",
                TargetLanguage = "Inglés"
            });

        modelBuilder.Entity<Lesson>().HasData(
            new Lesson { Id = 1, Title = "Fundamentos del Inglés", Subtitle = "Saluda y presenta", Status = "En progreso", XPReward = 50, ProgressPercent = 60 },
            new Lesson { Id = 2, Title = "Vocabulario básico", Subtitle = "Objetos y lugares", Status = "Desbloqueado", XPReward = 40, ProgressPercent = 0 },
            new Lesson { Id = 3, Title = "Pronombres y preguntas", Subtitle = "¿Quién? ¿Qué?", Status = "Bloqueado", XPReward = 45, ProgressPercent = 0 },
            new Lesson { Id = 4, Title = "Gramática inicial", Subtitle = "Ser, estar, tener", Status = "Bloqueado", XPReward = 60, ProgressPercent = 0 });
    }
}

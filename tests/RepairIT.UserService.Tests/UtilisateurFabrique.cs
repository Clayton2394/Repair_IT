using Microsoft.EntityFrameworkCore;
using Moq;
using RepairIT.Shared.Dtos.Utilisateurs;
using RepairIT.Shared.Enums;
using RepairIT.UserService.Data;
using RepairIT.UserService.Models;

namespace RepairIT.UserService.Tests;

/// <summary>Données et objets réutilisés par les tests des utilisateurs.</summary>
internal static class UtilisateurFabrique
{
    public static readonly DateTime DateFixe = new(2026, 11, 20, 10, 0, 0, DateTimeKind.Utc);

    public static Utilisateur Utilisateur(int id = 1, string nom = "Martin", string email = "alice@repairit.fr",
        RoleUtilisateur role = RoleUtilisateur.Gestionnaire, bool estActif = true) => new()
    {
        Id = id,
        Nom = nom,
        Prenom = "Alice",
        Email = email,
        Role = role,
        EstActif = estActif,
        DateCreation = DateFixe
    };

    public static UtilisateurCreationDto Creation(string email = "  Hugo.Bernard@RepairIT.fr ") => new()
    {
        Nom = " Bernard ",
        Prenom = " Hugo",
        Email = email,
        Role = RoleUtilisateur.Administrateur
    };

    public static TimeProvider HorlogeFixe()
    {
        var horloge = new Mock<TimeProvider>();
        horloge.Setup(h => h.GetUtcNow()).Returns(new DateTimeOffset(DateFixe));
        return horloge.Object;
    }

    /// <summary>Crée un contexte EF Core en mémoire, isolé pour chaque test.</summary>
    public static UtilisateursDbContext ContexteEnMemoire() =>
        new(new DbContextOptionsBuilder<UtilisateursDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}

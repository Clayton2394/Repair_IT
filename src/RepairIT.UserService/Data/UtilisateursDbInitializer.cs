using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RepairIT.Shared.Configuration;
using RepairIT.Shared.Enums;
using RepairIT.UserService.Models;

namespace RepairIT.UserService.Data;

/// <summary>Crée la base et insère les utilisateurs de démonstration au démarrage du service.</summary>
public class UtilisateursDbInitializer(
    UtilisateursDbContext contexte,
    IOptions<BaseDeDonneesOptions> options,
    ILogger<UtilisateursDbInitializer> logger)
{
    public async Task InitialiserAsync()
    {
        if (!options.Value.CreerAuDemarrage)
        {
            logger.LogInformation("Initialisation de la base désactivée par configuration.");
            return;
        }

        await contexte.Database.EnsureCreatedAsync();
        logger.LogInformation("Base de données des utilisateurs prête.");

        if (options.Value.InjecterDonneesDeDemo && !await contexte.Utilisateurs.AnyAsync())
        {
            contexte.Utilisateurs.AddRange(CreerUtilisateursDeDemo(DateTime.UtcNow));
            await contexte.SaveChangesAsync();
            logger.LogInformation("Utilisateurs de démonstration insérés.");
        }
    }

    /// <summary>1 administrateur et 4 gestionnaires, comme demandé pour la V5 (Keycloak).</summary>
    public static IReadOnlyList<Utilisateur> CreerUtilisateursDeDemo(DateTime dateCreation) =>
    [
        Nouveau("Martin", "Alice", "alice.martin@repairit.fr", RoleUtilisateur.Administrateur, dateCreation),
        Nouveau("Bernard", "Hugo", "hugo.bernard@repairit.fr", RoleUtilisateur.Gestionnaire, dateCreation),
        Nouveau("Petit", "Chloé", "chloe.petit@repairit.fr", RoleUtilisateur.Gestionnaire, dateCreation),
        Nouveau("Durand", "Lucas", "lucas.durand@repairit.fr", RoleUtilisateur.Gestionnaire, dateCreation),
        Nouveau("Leroy", "Emma", "emma.leroy@repairit.fr", RoleUtilisateur.Gestionnaire, dateCreation)
    ];

    private static Utilisateur Nouveau(string nom, string prenom, string email, RoleUtilisateur role,
        DateTime dateCreation) => new()
    {
        Nom = nom,
        Prenom = prenom,
        Email = email,
        Role = role,
        DateCreation = dateCreation
    };
}

using RepairIT.Shared.Dtos.Utilisateurs;
using RepairIT.Shared.Enums;
using RepairIT.Shared.Export;
using RepairIT.UserService.Models;
using RepairIT.UserService.Repositories;

namespace RepairIT.UserService.Services;

public class UtilisateurService(
    IUtilisateurRepository repository,
    TimeProvider horloge,
    ILogger<UtilisateurService> logger) : IUtilisateurService
{
    private static readonly IReadOnlyList<CsvColonne<UtilisateurDto>> ColonnesExport =
    [
        new("Id", u => u.Id),
        new("Nom", u => u.Nom),
        new("Prénom", u => u.Prenom),
        new("Email", u => u.Email),
        new("Rôle", u => u.Role),
        new("Actif", u => u.EstActif ? "Oui" : "Non"),
        new("Id Keycloak", u => u.UserKeycloak),
        new("Date de création", u => u.DateCreation)
    ];

    public async Task<IReadOnlyList<UtilisateurDto>> ListerAsync()
    {
        var utilisateurs = await repository.ListerAsync();
        return utilisateurs.Select(u => u.VersDto()).ToList();
    }

    public async Task<UtilisateurDto?> ObtenirAsync(int id)
    {
        var utilisateur = await repository.TrouverParIdAsync(id);
        return utilisateur?.VersDto();
    }

    public async Task<UtilisateurDto> CreerAsync(UtilisateurCreationDto saisie)
    {
        var utilisateur = saisie.VersEntite(horloge.GetUtcNow().UtcDateTime);

        if (await repository.EmailExisteAsync(utilisateur.Email))
        {
            logger.LogWarning("Création refusée : l'email {Email} existe déjà.", utilisateur.Email);
            throw new EmailDejaUtiliseException(utilisateur.Email);
        }

        await repository.AjouterAsync(utilisateur);
        logger.LogInformation("Utilisateur {Id} créé avec le rôle {Role}.", utilisateur.Id, utilisateur.Role);
        return utilisateur.VersDto();
    }

    public Task<UtilisateurDto?> AttribuerRoleAsync(int id, RoleUtilisateur role) =>
        ModifierAsync(id, utilisateur => utilisateur.Role = role, $"rôle {role} attribué");

    public Task<UtilisateurDto?> ChangerActivationAsync(int id, bool estActif) =>
        ModifierAsync(id, utilisateur => utilisateur.EstActif = estActif, estActif ? "compte activé" : "compte désactivé");

    public async Task<string> ExporterCsvAsync()
    {
        var utilisateurs = await ListerAsync();
        logger.LogInformation("Export CSV des utilisateurs ({Nombre} lignes).", utilisateurs.Count);
        return CsvExporter.Exporter(utilisateurs, ColonnesExport);
    }

    /// <summary>Charge un utilisateur, applique une modification et la sauvegarde.</summary>
    private async Task<UtilisateurDto?> ModifierAsync(int id, Action<Utilisateur> modification, string description)
    {
        var utilisateur = await repository.TrouverParIdAsync(id);
        if (utilisateur is null)
        {
            logger.LogWarning("Utilisateur {Id} introuvable ({Description}).", id, description);
            return null;
        }

        modification(utilisateur);
        await repository.MettreAJourAsync(utilisateur);
        logger.LogInformation("Utilisateur {Id} : {Description}.", id, description);
        return utilisateur.VersDto();
    }
}

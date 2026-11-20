using RepairIT.Shared.Dtos.Utilisateurs;
using RepairIT.Shared.Enums;

namespace RepairIT.UserService.Services;

/// <summary>Règles métier de gestion des utilisateurs.</summary>
public interface IUtilisateurService
{
    Task<IReadOnlyList<UtilisateurDto>> ListerAsync();
    Task<UtilisateurDto?> ObtenirAsync(int id);

    /// <exception cref="EmailDejaUtiliseException">Si l'email est déjà enregistré.</exception>
    Task<UtilisateurDto> CreerAsync(UtilisateurCreationDto saisie);

    Task<UtilisateurDto?> AttribuerRoleAsync(int id, RoleUtilisateur role);
    Task<UtilisateurDto?> ChangerActivationAsync(int id, bool estActif);
    Task<string> ExporterCsvAsync();
}

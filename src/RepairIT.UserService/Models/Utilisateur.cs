using RepairIT.Shared.Enums;

namespace RepairIT.UserService.Models;

/// <summary>
/// Utilisateur de l'application. Aucun mot de passe n'est stocké :
/// l'authentification sera déléguée à Keycloak (V5).
/// </summary>
public class Utilisateur
{
    /// <summary>Valeur utilisée tant que le compte n'est pas relié à Keycloak.</summary>
    public const string KeycloakNonSynchronise = "NON_SYNCHRONISE";

    public int Id { get; set; }
    public string UserKeycloak { get; set; } = KeycloakNonSynchronise;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public RoleUtilisateur Role { get; set; } = RoleUtilisateur.Gestionnaire;
    public bool EstActif { get; set; } = true;
    public DateTime DateCreation { get; set; }
}

using System.ComponentModel.DataAnnotations;
using RepairIT.Shared.Enums;

namespace RepairIT.Shared.Dtos.Utilisateurs;

/// <summary>Données saisies pour créer un utilisateur (aucun mot de passe : géré par Keycloak).</summary>
public class UtilisateurCreationDto
{
    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [StringLength(50)]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom est obligatoire.")]
    [StringLength(50)]
    public string Prenom { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email est obligatoire.")]
    [EmailAddress(ErrorMessage = "L'email n'est pas valide.")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    public RoleUtilisateur Role { get; set; } = RoleUtilisateur.Gestionnaire;
}

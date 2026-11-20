using RepairIT.Shared.Enums;

namespace RepairIT.Shared.Dtos.Utilisateurs;

/// <summary>Représentation d'un utilisateur renvoyée par l'API utilisateurs.</summary>
public record UtilisateurDto(
    int Id,
    string UserKeycloak,
    string Nom,
    string Prenom,
    string Email,
    RoleUtilisateur Role,
    bool EstActif,
    DateTime DateCreation);

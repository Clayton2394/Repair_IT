using RepairIT.Shared.Dtos.Utilisateurs;
using RepairIT.UserService.Models;

namespace RepairIT.UserService.Services;

/// <summary>Conversions entre l'entité <see cref="Utilisateur"/> et ses DTO.</summary>
public static class UtilisateurMapper
{
    public static UtilisateurDto VersDto(this Utilisateur u) => new(
        u.Id, u.UserKeycloak, u.Nom, u.Prenom, u.Email, u.Role, u.EstActif, u.DateCreation);

    public static Utilisateur VersEntite(this UtilisateurCreationDto saisie, DateTime dateCreation) => new()
    {
        Nom = saisie.Nom.Trim(),
        Prenom = saisie.Prenom.Trim(),
        Email = NormaliserEmail(saisie.Email),
        Role = saisie.Role,
        EstActif = true,
        DateCreation = dateCreation
    };

    public static string NormaliserEmail(string email) => email.Trim().ToLowerInvariant();
}

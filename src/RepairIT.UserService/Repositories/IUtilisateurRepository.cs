using RepairIT.UserService.Models;

namespace RepairIT.UserService.Repositories;

/// <summary>Accès aux données des utilisateurs.</summary>
public interface IUtilisateurRepository
{
    Task<IReadOnlyList<Utilisateur>> ListerAsync();
    Task<Utilisateur?> TrouverParIdAsync(int id);
    Task<bool> EmailExisteAsync(string email);
    Task<Utilisateur> AjouterAsync(Utilisateur utilisateur);
    Task MettreAJourAsync(Utilisateur utilisateur);
}

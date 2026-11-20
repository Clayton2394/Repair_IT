using Microsoft.EntityFrameworkCore;
using RepairIT.UserService.Data;
using RepairIT.UserService.Models;

namespace RepairIT.UserService.Repositories;

public class UtilisateurRepository(UtilisateursDbContext contexte) : IUtilisateurRepository
{
    public async Task<IReadOnlyList<Utilisateur>> ListerAsync() =>
        await contexte.Utilisateurs
            .AsNoTracking()
            .OrderBy(u => u.Nom)
            .ThenBy(u => u.Prenom)
            .ToListAsync();

    public Task<Utilisateur?> TrouverParIdAsync(int id) =>
        contexte.Utilisateurs.FirstOrDefaultAsync(u => u.Id == id);

    public Task<bool> EmailExisteAsync(string email) =>
        contexte.Utilisateurs.AnyAsync(u => u.Email == email);

    public async Task<Utilisateur> AjouterAsync(Utilisateur utilisateur)
    {
        contexte.Utilisateurs.Add(utilisateur);
        await contexte.SaveChangesAsync();
        return utilisateur;
    }

    public async Task MettreAJourAsync(Utilisateur utilisateur)
    {
        contexte.Utilisateurs.Update(utilisateur);
        await contexte.SaveChangesAsync();
    }
}

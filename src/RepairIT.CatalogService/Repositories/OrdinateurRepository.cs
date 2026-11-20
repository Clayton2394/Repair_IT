using Microsoft.EntityFrameworkCore;
using RepairIT.CatalogService.Data;
using RepairIT.CatalogService.Models;

namespace RepairIT.CatalogService.Repositories;

public class OrdinateurRepository(CatalogueDbContext contexte) : IOrdinateurRepository
{
    public async Task<IReadOnlyList<Ordinateur>> ListerAsync(string? recherche = null)
    {
        var requete = contexte.Ordinateurs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(recherche))
        {
            var terme = recherche.Trim().ToLower();
            requete = requete.Where(o =>
                o.Marque.ToLower().Contains(terme) ||
                o.Modele.ToLower().Contains(terme) ||
                o.Processeur.ToLower().Contains(terme));
        }

        return await requete.OrderBy(o => o.Marque).ThenBy(o => o.Modele).ToListAsync();
    }

    public Task<Ordinateur?> TrouverParIdAsync(int id) =>
        contexte.Ordinateurs.FirstOrDefaultAsync(o => o.Id == id);

    public async Task<Ordinateur> AjouterAsync(Ordinateur ordinateur)
    {
        contexte.Ordinateurs.Add(ordinateur);
        await contexte.SaveChangesAsync();
        return ordinateur;
    }

    public async Task MettreAJourAsync(Ordinateur ordinateur)
    {
        contexte.Ordinateurs.Update(ordinateur);
        await contexte.SaveChangesAsync();
    }

    public async Task SupprimerAsync(Ordinateur ordinateur)
    {
        contexte.Ordinateurs.Remove(ordinateur);
        await contexte.SaveChangesAsync();
    }
}

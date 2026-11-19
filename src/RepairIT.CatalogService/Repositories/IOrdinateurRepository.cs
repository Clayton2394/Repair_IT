using RepairIT.CatalogService.Models;

namespace RepairIT.CatalogService.Repositories;

/// <summary>Accès aux données des ordinateurs (isole EF Core du reste du service).</summary>
public interface IOrdinateurRepository
{
    Task<IReadOnlyList<Ordinateur>> ListerAsync(string? recherche = null);
    Task<Ordinateur?> TrouverParIdAsync(int id);
    Task<Ordinateur> AjouterAsync(Ordinateur ordinateur);
    Task MettreAJourAsync(Ordinateur ordinateur);
    Task SupprimerAsync(Ordinateur ordinateur);
}

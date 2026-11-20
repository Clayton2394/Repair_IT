using RepairIT.Shared.Dtos.Ordinateurs;

namespace RepairIT.CatalogService.Services;

/// <summary>Règles métier de gestion du catalogue d'ordinateurs.</summary>
public interface IOrdinateurService
{
    Task<IReadOnlyList<OrdinateurDto>> ListerAsync(string? recherche = null);
    Task<OrdinateurDto?> ObtenirAsync(int id);
    Task<OrdinateurDto> CreerAsync(OrdinateurSaisieDto saisie);
    Task<OrdinateurDto?> ModifierAsync(int id, OrdinateurSaisieDto saisie);
    Task<bool> SupprimerAsync(int id);
    Task<EtatStockDto> ConsulterStockAsync();
    Task<string> ExporterCsvAsync();
}

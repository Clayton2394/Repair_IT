using Microsoft.Extensions.Options;
using RepairIT.CatalogService.Configuration;
using RepairIT.CatalogService.Repositories;
using RepairIT.Shared.Dtos.Ordinateurs;
using RepairIT.Shared.Export;

namespace RepairIT.CatalogService.Services;

public class OrdinateurService(
    IOrdinateurRepository repository,
    IOptions<CatalogueOptions> options,
    TimeProvider horloge,
    ILogger<OrdinateurService> logger) : IOrdinateurService
{
    private static readonly IReadOnlyList<CsvColonne<OrdinateurDto>> ColonnesExport =
    [
        new("Id", o => o.Id),
        new("Marque", o => o.Marque),
        new("Modèle", o => o.Modele),
        new("Processeur", o => o.Processeur),
        new("RAM (Go)", o => o.MemoireRamGo),
        new("Stockage (Go)", o => o.StockageGo),
        new("Prix (€)", o => o.Prix),
        new("Quantité", o => o.QuantiteEnStock),
        new("État", o => o.Etat),
        new("Date d'ajout", o => o.DateAjout)
    ];

    public async Task<IReadOnlyList<OrdinateurDto>> ListerAsync(string? recherche = null)
    {
        var ordinateurs = await repository.ListerAsync(recherche);
        return ordinateurs.Select(o => o.VersDto()).ToList();
    }

    public async Task<OrdinateurDto?> ObtenirAsync(int id)
    {
        var ordinateur = await repository.TrouverParIdAsync(id);
        return ordinateur?.VersDto();
    }

    public async Task<OrdinateurDto> CreerAsync(OrdinateurSaisieDto saisie)
    {
        var ordinateur = saisie.VersEntite(horloge.GetUtcNow().UtcDateTime);
        await repository.AjouterAsync(ordinateur);
        logger.LogInformation("Ordinateur {Id} ({Marque} {Modele}) ajouté au catalogue.",
            ordinateur.Id, ordinateur.Marque, ordinateur.Modele);
        return ordinateur.VersDto();
    }

    public async Task<OrdinateurDto?> ModifierAsync(int id, OrdinateurSaisieDto saisie)
    {
        var ordinateur = await repository.TrouverParIdAsync(id);
        if (ordinateur is null)
        {
            logger.LogWarning("Modification impossible : ordinateur {Id} introuvable.", id);
            return null;
        }

        saisie.AppliquerA(ordinateur);
        await repository.MettreAJourAsync(ordinateur);
        logger.LogInformation("Ordinateur {Id} mis à jour.", id);
        return ordinateur.VersDto();
    }

    public async Task<bool> SupprimerAsync(int id)
    {
        var ordinateur = await repository.TrouverParIdAsync(id);
        if (ordinateur is null)
        {
            logger.LogWarning("Suppression impossible : ordinateur {Id} introuvable.", id);
            return false;
        }

        await repository.SupprimerAsync(ordinateur);
        logger.LogInformation("Ordinateur {Id} supprimé du catalogue.", id);
        return true;
    }

    public async Task<EtatStockDto> ConsulterStockAsync()
    {
        var ordinateurs = await ListerAsync();
        var seuil = options.Value.SeuilStockFaible;

        var stockFaible = ordinateurs
            .Where(o => o.QuantiteEnStock <= seuil)
            .OrderBy(o => o.QuantiteEnStock)
            .ToList();

        return new EtatStockDto(
            NombreDeReferences: ordinateurs.Count,
            NombreTotalUnites: ordinateurs.Sum(o => o.QuantiteEnStock),
            ValeurTotaleStock: ordinateurs.Sum(o => o.Prix * o.QuantiteEnStock),
            SeuilStockFaible: seuil,
            ReferencesEnStockFaible: stockFaible);
    }

    public async Task<string> ExporterCsvAsync()
    {
        var ordinateurs = await ListerAsync();
        logger.LogInformation("Export CSV du catalogue ({Nombre} lignes).", ordinateurs.Count);
        return CsvExporter.Exporter(ordinateurs, ColonnesExport);
    }
}

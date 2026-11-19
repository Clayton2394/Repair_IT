using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RepairIT.Shared.Configuration;
using RepairIT.CatalogService.Models;
using RepairIT.Shared.Enums;

namespace RepairIT.CatalogService.Data;

/// <summary>Crée la base et insère les données de démonstration au démarrage du service.</summary>
public class CatalogueDbInitializer(
    CatalogueDbContext contexte,
    IOptions<BaseDeDonneesOptions> options,
    ILogger<CatalogueDbInitializer> logger)
{
    public async Task InitialiserAsync()
    {
        if (!options.Value.CreerAuDemarrage)
        {
            logger.LogInformation("Initialisation de la base désactivée par configuration.");
            return;
        }

        await contexte.Database.EnsureCreatedAsync();
        logger.LogInformation("Base de données du catalogue prête.");

        if (options.Value.InjecterDonneesDeDemo && !await contexte.Ordinateurs.AnyAsync())
        {
            contexte.Ordinateurs.AddRange(CreerOrdinateursDeDemo(DateTime.UtcNow));
            await contexte.SaveChangesAsync();
            logger.LogInformation("Données de démonstration du catalogue insérées.");
        }
    }

    public static IReadOnlyList<Ordinateur> CreerOrdinateursDeDemo(DateTime dateAjout) =>
    [
        Nouveau("Dell", "XPS 13 9340", "Intel Core Ultra 7 155H", 16, 512, 1499m, 5, EtatOrdinateur.Neuf, dateAjout),
        Nouveau("Apple", "MacBook Air M3", "Apple M3", 16, 512, 1399m, 8, EtatOrdinateur.Neuf, dateAjout),
        Nouveau("Lenovo", "ThinkPad T14 Gen 4", "AMD Ryzen 7 PRO 7840U", 32, 1024, 1249m, 2, EtatOrdinateur.Neuf, dateAjout),
        Nouveau("HP", "EliteBook 840 G8", "Intel Core i5-1135G7", 16, 256, 549m, 4, EtatOrdinateur.Reconditionne, dateAjout),
        Nouveau("Asus", "ROG Strix G16", "Intel Core i9-14900HX", 32, 1024, 2199m, 1, EtatOrdinateur.Neuf, dateAjout),
        Nouveau("Acer", "Aspire 5", "Intel Core i3-1215U", 8, 256, 299m, 3, EtatOrdinateur.Occasion, dateAjout)
    ];

    private static Ordinateur Nouveau(string marque, string modele, string processeur, int ram, int stockage,
        decimal prix, int quantite, EtatOrdinateur etat, DateTime dateAjout) => new()
    {
        Marque = marque,
        Modele = modele,
        Processeur = processeur,
        MemoireRamGo = ram,
        StockageGo = stockage,
        Prix = prix,
        QuantiteEnStock = quantite,
        Etat = etat,
        DateAjout = dateAjout
    };
}

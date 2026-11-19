using Microsoft.EntityFrameworkCore;
using Moq;
using RepairIT.CatalogService.Data;
using RepairIT.CatalogService.Models;
using RepairIT.Shared.Dtos.Ordinateurs;
using RepairIT.Shared.Enums;

namespace RepairIT.CatalogService.Tests;

/// <summary>Données et objets réutilisés par les tests du catalogue.</summary>
internal static class OrdinateurFabrique
{
    public static readonly DateTime DateFixe = new(2026, 11, 20, 10, 0, 0, DateTimeKind.Utc);

    public static Ordinateur Ordinateur(int id = 1, string marque = "Dell", string modele = "XPS 13",
        decimal prix = 1000m, int quantite = 5) => new()
    {
        Id = id,
        Marque = marque,
        Modele = modele,
        Processeur = "Intel Core i7",
        MemoireRamGo = 16,
        StockageGo = 512,
        Prix = prix,
        QuantiteEnStock = quantite,
        Etat = EtatOrdinateur.Neuf,
        DateAjout = DateFixe
    };

    public static OrdinateurSaisieDto Saisie(string marque = "  Lenovo ", string modele = " ThinkPad ") => new()
    {
        Marque = marque,
        Modele = modele,
        Processeur = "AMD Ryzen 7",
        MemoireRamGo = 32,
        StockageGo = 1024,
        Prix = 1249m,
        QuantiteEnStock = 2,
        Etat = EtatOrdinateur.Reconditionne,
        Description = "   "
    };

    public static TimeProvider HorlogeFixe()
    {
        var horloge = new Mock<TimeProvider>();
        horloge.Setup(h => h.GetUtcNow()).Returns(new DateTimeOffset(DateFixe));
        return horloge.Object;
    }

    /// <summary>Crée un contexte EF Core en mémoire, isolé pour chaque test.</summary>
    public static CatalogueDbContext ContexteEnMemoire() =>
        new(new DbContextOptionsBuilder<CatalogueDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}

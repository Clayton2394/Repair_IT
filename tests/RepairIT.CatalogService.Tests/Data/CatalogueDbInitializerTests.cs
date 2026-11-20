using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using RepairIT.CatalogService.Data;
using RepairIT.Shared.Configuration;
using static RepairIT.CatalogService.Tests.OrdinateurFabrique;

namespace RepairIT.CatalogService.Tests.Data;

public class CatalogueDbInitializerTests
{
    private static CatalogueDbInitializer Initialiseur(CatalogueDbContext contexte, bool creer = true, bool demo = true) =>
        new(contexte,
            Options.Create(new BaseDeDonneesOptions { CreerAuDemarrage = creer, InjecterDonneesDeDemo = demo }),
            NullLogger<CatalogueDbInitializer>.Instance);

    [Fact]
    public async Task InitialiserAsync_BaseVide_InsereLesDonneesDeDemo()
    {
        using var contexte = ContexteEnMemoire();

        await Initialiseur(contexte).InitialiserAsync();

        Assert.Equal(CatalogueDbInitializer.CreerOrdinateursDeDemo(DateFixe).Count, contexte.Ordinateurs.Count());
    }

    [Fact]
    public async Task InitialiserAsync_BaseDejaRemplie_NeDupliquePas()
    {
        using var contexte = ContexteEnMemoire();
        contexte.Ordinateurs.Add(Ordinateur(id: 1));
        await contexte.SaveChangesAsync();

        await Initialiseur(contexte).InitialiserAsync();

        Assert.Equal(1, contexte.Ordinateurs.Count());
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task InitialiserAsync_DesactiveParConfiguration_NInsereRien(bool creer, bool demo)
    {
        using var contexte = ContexteEnMemoire();

        await Initialiseur(contexte, creer, demo).InitialiserAsync();

        Assert.Empty(contexte.Ordinateurs);
    }
}

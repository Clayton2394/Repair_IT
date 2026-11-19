using RepairIT.CatalogService.Data;
using RepairIT.CatalogService.Repositories;
using static RepairIT.CatalogService.Tests.OrdinateurFabrique;

namespace RepairIT.CatalogService.Tests.Repositories;

public class OrdinateurRepositoryTests : IDisposable
{
    private readonly CatalogueDbContext contexte = ContexteEnMemoire();
    private readonly OrdinateurRepository repository;

    public OrdinateurRepositoryTests()
    {
        repository = new OrdinateurRepository(contexte);
        contexte.Ordinateurs.AddRange(
            Ordinateur(id: 1, marque: "Lenovo", modele: "ThinkPad"),
            Ordinateur(id: 2, marque: "Apple", modele: "MacBook Pro"),
            Ordinateur(id: 3, marque: "Apple", modele: "MacBook Air"));
        contexte.SaveChanges();
        contexte.ChangeTracker.Clear();
    }

    [Fact]
    public async Task ListerAsync_TrieParMarquePuisModele()
    {
        var ordinateurs = await repository.ListerAsync();

        Assert.Equal([3, 2, 1], ordinateurs.Select(o => o.Id));
    }

    [Theory]
    [InlineData("macbook", 2)]
    [InlineData("LENOVO", 1)]
    [InlineData("intel", 3)]
    [InlineData("inconnu", 0)]
    public async Task ListerAsync_FiltreSansTenirCompteDeLaCasse(string recherche, int nombreAttendu)
    {
        var ordinateurs = await repository.ListerAsync(recherche);

        Assert.Equal(nombreAttendu, ordinateurs.Count);
    }

    [Fact]
    public async Task AjouterAsync_AttribueUnIdentifiant()
    {
        var nouveau = Ordinateur(id: 0, marque: "Asus");

        var ajoute = await repository.AjouterAsync(nouveau);

        Assert.True(ajoute.Id > 0);
        Assert.Equal(4, contexte.Ordinateurs.Count());
    }

    [Fact]
    public async Task MettreAJourAsync_PersisteLesModifications()
    {
        var ordinateur = (await repository.TrouverParIdAsync(1))!;
        ordinateur.QuantiteEnStock = 42;

        await repository.MettreAJourAsync(ordinateur);
        contexte.ChangeTracker.Clear();

        Assert.Equal(42, (await repository.TrouverParIdAsync(1))!.QuantiteEnStock);
    }

    [Fact]
    public async Task SupprimerAsync_RetireLOrdinateur()
    {
        var ordinateur = (await repository.TrouverParIdAsync(2))!;

        await repository.SupprimerAsync(ordinateur);

        Assert.Null(await repository.TrouverParIdAsync(2));
    }

    public void Dispose() => contexte.Dispose();
}

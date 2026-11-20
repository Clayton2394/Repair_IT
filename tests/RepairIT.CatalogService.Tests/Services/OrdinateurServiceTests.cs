using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using RepairIT.CatalogService.Configuration;
using RepairIT.CatalogService.Models;
using RepairIT.CatalogService.Repositories;
using RepairIT.CatalogService.Services;
using RepairIT.Shared.Enums;
using static RepairIT.CatalogService.Tests.OrdinateurFabrique;

namespace RepairIT.CatalogService.Tests.Services;

public class OrdinateurServiceTests
{
    private readonly Mock<IOrdinateurRepository> repository = new();
    private readonly OrdinateurService service;

    public OrdinateurServiceTests()
    {
        var options = Options.Create(new CatalogueOptions { SeuilStockFaible = 2 });
        service = new OrdinateurService(repository.Object, options, HorlogeFixe(), NullLogger<OrdinateurService>.Instance);
    }

    [Fact]
    public async Task ListerAsync_TransmetLaRechercheEtConvertitEnDto()
    {
        repository.Setup(r => r.ListerAsync("dell")).ReturnsAsync([Ordinateur(id: 7)]);

        var resultat = await service.ListerAsync("dell");

        var dto = Assert.Single(resultat);
        Assert.Equal(7, dto.Id);
        Assert.Equal("Dell", dto.Marque);
    }

    [Fact]
    public async Task ObtenirAsync_OrdinateurInexistant_RenvoieNull()
    {
        repository.Setup(r => r.TrouverParIdAsync(99)).ReturnsAsync((Ordinateur?)null);

        Assert.Null(await service.ObtenirAsync(99));
    }

    [Fact]
    public async Task ObtenirAsync_OrdinateurExistant_RenvoieLeDto()
    {
        repository.Setup(r => r.TrouverParIdAsync(1)).ReturnsAsync(Ordinateur(id: 1, modele: "Latitude"));

        var dto = await service.ObtenirAsync(1);

        Assert.Equal("Latitude", dto?.Modele);
    }

    [Fact]
    public async Task CreerAsync_NettoieLaSaisieEtDateLAjout()
    {
        Ordinateur? enregistre = null;
        repository.Setup(r => r.AjouterAsync(It.IsAny<Ordinateur>()))
            .Callback<Ordinateur>(o => enregistre = o)
            .ReturnsAsync((Ordinateur o) => o);

        var dto = await service.CreerAsync(Saisie());

        Assert.NotNull(enregistre);
        Assert.Equal("Lenovo", enregistre.Marque);
        Assert.Equal("ThinkPad", enregistre.Modele);
        Assert.Null(enregistre.Description);
        Assert.Equal(DateFixe, enregistre.DateAjout);
        Assert.Equal(EtatOrdinateur.Reconditionne, dto.Etat);
    }

    [Fact]
    public async Task ModifierAsync_OrdinateurInexistant_RenvoieNullSansSauvegarder()
    {
        repository.Setup(r => r.TrouverParIdAsync(5)).ReturnsAsync((Ordinateur?)null);

        var resultat = await service.ModifierAsync(5, Saisie());

        Assert.Null(resultat);
        repository.Verify(r => r.MettreAJourAsync(It.IsAny<Ordinateur>()), Times.Never);
    }

    [Fact]
    public async Task ModifierAsync_OrdinateurExistant_AppliqueLesChangementsEtConserveLaDateDAjout()
    {
        var existant = Ordinateur(id: 3);
        existant.DateAjout = new DateTime(2025, 1, 1);
        repository.Setup(r => r.TrouverParIdAsync(3)).ReturnsAsync(existant);

        var resultat = await service.ModifierAsync(3, Saisie());

        Assert.Equal("Lenovo", resultat?.Marque);
        Assert.Equal(1249m, existant.Prix);
        Assert.Equal(new DateTime(2025, 1, 1), existant.DateAjout);
        repository.Verify(r => r.MettreAJourAsync(existant), Times.Once);
    }

    [Fact]
    public async Task SupprimerAsync_OrdinateurExistant_SupprimeEtRenvoieVrai()
    {
        var existant = Ordinateur(id: 4);
        repository.Setup(r => r.TrouverParIdAsync(4)).ReturnsAsync(existant);

        Assert.True(await service.SupprimerAsync(4));
        repository.Verify(r => r.SupprimerAsync(existant), Times.Once);
    }

    [Fact]
    public async Task SupprimerAsync_OrdinateurInexistant_RenvoieFaux()
    {
        repository.Setup(r => r.TrouverParIdAsync(4)).ReturnsAsync((Ordinateur?)null);

        Assert.False(await service.SupprimerAsync(4));
        repository.Verify(r => r.SupprimerAsync(It.IsAny<Ordinateur>()), Times.Never);
    }

    [Fact]
    public async Task ConsulterStockAsync_CalculeLesTotauxEtLesAlertes()
    {
        repository.Setup(r => r.ListerAsync(null)).ReturnsAsync(
        [
            Ordinateur(id: 1, prix: 1000m, quantite: 5),
            Ordinateur(id: 2, prix: 500m, quantite: 2),
            Ordinateur(id: 3, prix: 200m, quantite: 0)
        ]);

        var stock = await service.ConsulterStockAsync();

        Assert.Equal(3, stock.NombreDeReferences);
        Assert.Equal(7, stock.NombreTotalUnites);
        Assert.Equal(6000m, stock.ValeurTotaleStock);
        Assert.Equal(2, stock.SeuilStockFaible);
        Assert.Equal([3, 2], stock.ReferencesEnStockFaible.Select(o => o.Id));
    }

    [Fact]
    public async Task ExporterCsvAsync_ContientLEnTeteEtLesOrdinateurs()
    {
        repository.Setup(r => r.ListerAsync(null)).ReturnsAsync([Ordinateur(marque: "Apple", modele: "MacBook Air")]);

        var csv = await service.ExporterCsvAsync();

        Assert.StartsWith("Id;Marque;Modèle", csv);
        Assert.Contains("Apple;MacBook Air", csv);
    }
}

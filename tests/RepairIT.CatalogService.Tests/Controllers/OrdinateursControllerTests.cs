using Microsoft.AspNetCore.Mvc;
using Moq;
using RepairIT.CatalogService.Controllers;
using RepairIT.CatalogService.Services;
using RepairIT.Shared.Dtos.Ordinateurs;
using static RepairIT.CatalogService.Tests.OrdinateurFabrique;

namespace RepairIT.CatalogService.Tests.Controllers;

public class OrdinateursControllerTests
{
    private readonly Mock<IOrdinateurService> service = new();
    private readonly OrdinateursController controleur;

    public OrdinateursControllerTests() => controleur = new OrdinateursController(service.Object);

    private static OrdinateurDto Dto(int id = 1) => Ordinateur(id).VersDto();

    [Fact]
    public async Task Lister_RenvoieOkAvecLaListe()
    {
        service.Setup(s => s.ListerAsync("hp")).ReturnsAsync([Dto()]);

        var resultat = await controleur.Lister("hp");

        var ok = Assert.IsType<OkObjectResult>(resultat.Result);
        Assert.Single(Assert.IsAssignableFrom<IReadOnlyList<OrdinateurDto>>(ok.Value));
    }

    [Fact]
    public async Task Obtenir_Inexistant_RenvoieNotFound()
    {
        service.Setup(s => s.ObtenirAsync(9)).ReturnsAsync((OrdinateurDto?)null);

        var resultat = await controleur.Obtenir(9);

        Assert.IsType<NotFoundResult>(resultat.Result);
    }

    [Fact]
    public async Task Obtenir_Existant_RenvoieOk()
    {
        service.Setup(s => s.ObtenirAsync(1)).ReturnsAsync(Dto());

        var resultat = await controleur.Obtenir(1);

        Assert.IsType<OkObjectResult>(resultat.Result);
    }

    [Fact]
    public async Task Creer_RenvoieCreatedAvecLaRouteDeConsultation()
    {
        service.Setup(s => s.CreerAsync(It.IsAny<OrdinateurSaisieDto>())).ReturnsAsync(Dto(12));

        var resultat = await controleur.Creer(Saisie());

        var cree = Assert.IsType<CreatedAtActionResult>(resultat.Result);
        Assert.Equal(nameof(OrdinateursController.Obtenir), cree.ActionName);
        Assert.Equal(12, cree.RouteValues!["id"]);
    }

    [Fact]
    public async Task Modifier_Inexistant_RenvoieNotFound()
    {
        service.Setup(s => s.ModifierAsync(3, It.IsAny<OrdinateurSaisieDto>())).ReturnsAsync((OrdinateurDto?)null);

        var resultat = await controleur.Modifier(3, Saisie());

        Assert.IsType<NotFoundResult>(resultat.Result);
    }

    [Fact]
    public async Task Modifier_Existant_RenvoieOk()
    {
        service.Setup(s => s.ModifierAsync(3, It.IsAny<OrdinateurSaisieDto>())).ReturnsAsync(Dto(3));

        var resultat = await controleur.Modifier(3, Saisie());

        Assert.IsType<OkObjectResult>(resultat.Result);
    }

    [Theory]
    [InlineData(true, typeof(NoContentResult))]
    [InlineData(false, typeof(NotFoundResult))]
    public async Task Supprimer_RenvoieLeCodeAdapte(bool supprime, Type typeAttendu)
    {
        service.Setup(s => s.SupprimerAsync(1)).ReturnsAsync(supprime);

        var resultat = await controleur.Supprimer(1);

        Assert.IsType(typeAttendu, resultat);
    }

    [Fact]
    public async Task ConsulterStock_RenvoieOk()
    {
        service.Setup(s => s.ConsulterStockAsync()).ReturnsAsync(new EtatStockDto(0, 0, 0, 3, []));

        var resultat = await controleur.ConsulterStock();

        Assert.IsType<OkObjectResult>(resultat.Result);
    }

    [Fact]
    public async Task Exporter_RenvoieUnFichierCsv()
    {
        service.Setup(s => s.ExporterCsvAsync()).ReturnsAsync("Id;Marque\n");

        var resultat = await controleur.Exporter();

        var fichier = Assert.IsType<FileContentResult>(resultat);
        Assert.Equal("text/csv", fichier.ContentType);
        Assert.Equal("catalogue.csv", fichier.FileDownloadName);
    }
}

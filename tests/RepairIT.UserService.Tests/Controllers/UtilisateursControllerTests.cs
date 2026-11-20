using Microsoft.AspNetCore.Mvc;
using Moq;
using RepairIT.Shared.Dtos.Utilisateurs;
using RepairIT.Shared.Enums;
using RepairIT.UserService.Controllers;
using RepairIT.UserService.Services;
using static RepairIT.UserService.Tests.UtilisateurFabrique;

namespace RepairIT.UserService.Tests.Controllers;

public class UtilisateursControllerTests
{
    private readonly Mock<IUtilisateurService> service = new();
    private readonly UtilisateursController controleur;

    public UtilisateursControllerTests() => controleur = new UtilisateursController(service.Object);

    private static UtilisateurDto Dto(int id = 1) => Utilisateur(id).VersDto();

    [Fact]
    public async Task Lister_RenvoieOk()
    {
        service.Setup(s => s.ListerAsync()).ReturnsAsync([Dto()]);

        var resultat = await controleur.Lister();

        Assert.IsType<OkObjectResult>(resultat.Result);
    }

    [Fact]
    public async Task Obtenir_Inexistant_RenvoieNotFound()
    {
        service.Setup(s => s.ObtenirAsync(4)).ReturnsAsync((UtilisateurDto?)null);

        Assert.IsType<NotFoundResult>((await controleur.Obtenir(4)).Result);
    }

    [Fact]
    public async Task Obtenir_Existant_RenvoieOk()
    {
        service.Setup(s => s.ObtenirAsync(1)).ReturnsAsync(Dto());

        Assert.IsType<OkObjectResult>((await controleur.Obtenir(1)).Result);
    }

    [Fact]
    public async Task Creer_RenvoieCreated()
    {
        service.Setup(s => s.CreerAsync(It.IsAny<UtilisateurCreationDto>())).ReturnsAsync(Dto(6));

        var resultat = await controleur.Creer(Creation());

        var cree = Assert.IsType<CreatedAtActionResult>(resultat.Result);
        Assert.Equal(6, cree.RouteValues!["id"]);
    }

    [Fact]
    public async Task Creer_EmailDejaUtilise_RenvoieConflict()
    {
        service.Setup(s => s.CreerAsync(It.IsAny<UtilisateurCreationDto>()))
            .ThrowsAsync(new EmailDejaUtiliseException("hugo@repairit.fr"));

        var resultat = await controleur.Creer(Creation());

        var conflit = Assert.IsType<ConflictObjectResult>(resultat.Result);
        var probleme = Assert.IsType<ProblemDetails>(conflit.Value);
        Assert.Contains("hugo@repairit.fr", probleme.Detail);
    }

    [Fact]
    public async Task AttribuerRole_Inexistant_RenvoieNotFound()
    {
        service.Setup(s => s.AttribuerRoleAsync(2, RoleUtilisateur.Administrateur)).ReturnsAsync((UtilisateurDto?)null);

        var resultat = await controleur.AttribuerRole(2, new ChangementRoleDto(RoleUtilisateur.Administrateur));

        Assert.IsType<NotFoundResult>(resultat.Result);
    }

    [Fact]
    public async Task AttribuerRole_Existant_RenvoieOk()
    {
        service.Setup(s => s.AttribuerRoleAsync(2, RoleUtilisateur.Administrateur)).ReturnsAsync(Dto(2));

        var resultat = await controleur.AttribuerRole(2, new ChangementRoleDto(RoleUtilisateur.Administrateur));

        Assert.IsType<OkObjectResult>(resultat.Result);
    }

    [Fact]
    public async Task Activer_AppelleLeServiceAvecVrai()
    {
        service.Setup(s => s.ChangerActivationAsync(1, true)).ReturnsAsync(Dto());

        var resultat = await controleur.Activer(1);

        Assert.IsType<OkObjectResult>(resultat.Result);
        service.Verify(s => s.ChangerActivationAsync(1, true), Times.Once);
    }

    [Fact]
    public async Task Desactiver_Inexistant_RenvoieNotFound()
    {
        service.Setup(s => s.ChangerActivationAsync(1, false)).ReturnsAsync((UtilisateurDto?)null);

        var resultat = await controleur.Desactiver(1);

        Assert.IsType<NotFoundResult>(resultat.Result);
    }

    [Fact]
    public async Task Exporter_RenvoieUnFichierCsv()
    {
        service.Setup(s => s.ExporterCsvAsync()).ReturnsAsync("Id;Nom\n");

        var fichier = Assert.IsType<FileContentResult>(await controleur.Exporter());

        Assert.Equal("utilisateurs.csv", fichier.FileDownloadName);
    }
}

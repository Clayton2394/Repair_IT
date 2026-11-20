using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using RepairIT.Shared.Enums;
using RepairIT.UserService.Models;
using RepairIT.UserService.Repositories;
using RepairIT.UserService.Services;
using static RepairIT.UserService.Tests.UtilisateurFabrique;

namespace RepairIT.UserService.Tests.Services;

public class UtilisateurServiceTests
{
    private readonly Mock<IUtilisateurRepository> repository = new();
    private readonly UtilisateurService service;

    public UtilisateurServiceTests() =>
        service = new UtilisateurService(repository.Object, HorlogeFixe(), NullLogger<UtilisateurService>.Instance);

    [Fact]
    public async Task ListerAsync_ConvertitEnDto()
    {
        repository.Setup(r => r.ListerAsync()).ReturnsAsync([Utilisateur(id: 1), Utilisateur(id: 2)]);

        var utilisateurs = await service.ListerAsync();

        Assert.Equal([1, 2], utilisateurs.Select(u => u.Id));
    }

    [Fact]
    public async Task ObtenirAsync_Inexistant_RenvoieNull()
    {
        repository.Setup(r => r.TrouverParIdAsync(8)).ReturnsAsync((Utilisateur?)null);

        Assert.Null(await service.ObtenirAsync(8));
    }

    [Fact]
    public async Task ObtenirAsync_Existant_RenvoieLeDto()
    {
        repository.Setup(r => r.TrouverParIdAsync(1)).ReturnsAsync(Utilisateur(nom: "Durand"));

        Assert.Equal("Durand", (await service.ObtenirAsync(1))?.Nom);
    }

    [Fact]
    public async Task CreerAsync_NormaliseLaSaisieEtAppliqueLesValeursParDefaut()
    {
        Utilisateur? enregistre = null;
        repository.Setup(r => r.EmailExisteAsync(It.IsAny<string>())).ReturnsAsync(false);
        repository.Setup(r => r.AjouterAsync(It.IsAny<Utilisateur>()))
            .Callback<Utilisateur>(u => enregistre = u)
            .ReturnsAsync((Utilisateur u) => u);

        var dto = await service.CreerAsync(Creation());

        Assert.NotNull(enregistre);
        Assert.Equal("hugo.bernard@repairit.fr", enregistre.Email);
        Assert.Equal("Bernard", enregistre.Nom);
        Assert.Equal("Hugo", enregistre.Prenom);
        Assert.True(enregistre.EstActif);
        Assert.Equal(DateFixe, enregistre.DateCreation);
        Assert.Equal(UserService.Models.Utilisateur.KeycloakNonSynchronise, dto.UserKeycloak);
        Assert.Equal(RoleUtilisateur.Administrateur, dto.Role);
    }

    [Fact]
    public async Task CreerAsync_EmailDejaUtilise_LeveUneExceptionSansEnregistrer()
    {
        repository.Setup(r => r.EmailExisteAsync("hugo.bernard@repairit.fr")).ReturnsAsync(true);

        await Assert.ThrowsAsync<EmailDejaUtiliseException>(() => service.CreerAsync(Creation()));

        repository.Verify(r => r.AjouterAsync(It.IsAny<Utilisateur>()), Times.Never);
    }

    [Fact]
    public async Task AttribuerRoleAsync_Existant_ModifieLeRole()
    {
        var utilisateur = Utilisateur(role: RoleUtilisateur.Gestionnaire);
        repository.Setup(r => r.TrouverParIdAsync(1)).ReturnsAsync(utilisateur);

        var dto = await service.AttribuerRoleAsync(1, RoleUtilisateur.Administrateur);

        Assert.Equal(RoleUtilisateur.Administrateur, dto?.Role);
        repository.Verify(r => r.MettreAJourAsync(utilisateur), Times.Once);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task ChangerActivationAsync_Existant_ModifieLeStatut(bool etatInitial, bool nouvelEtat)
    {
        var utilisateur = Utilisateur(estActif: etatInitial);
        repository.Setup(r => r.TrouverParIdAsync(1)).ReturnsAsync(utilisateur);

        var dto = await service.ChangerActivationAsync(1, nouvelEtat);

        Assert.Equal(nouvelEtat, dto?.EstActif);
        repository.Verify(r => r.MettreAJourAsync(utilisateur), Times.Once);
    }

    [Fact]
    public async Task ChangerActivationAsync_Inexistant_RenvoieNullSansSauvegarder()
    {
        repository.Setup(r => r.TrouverParIdAsync(3)).ReturnsAsync((Utilisateur?)null);

        Assert.Null(await service.ChangerActivationAsync(3, false));
        repository.Verify(r => r.MettreAJourAsync(It.IsAny<Utilisateur>()), Times.Never);
    }

    [Fact]
    public async Task ExporterCsvAsync_ContientLesUtilisateurs()
    {
        repository.Setup(r => r.ListerAsync()).ReturnsAsync([Utilisateur(estActif: false)]);

        var csv = await service.ExporterCsvAsync();

        Assert.StartsWith("Id;Nom;Prénom;Email;Rôle;Actif", csv);
        Assert.Contains("Martin;Alice;alice@repairit.fr;Gestionnaire;Non;NON_SYNCHRONISE", csv);
    }
}

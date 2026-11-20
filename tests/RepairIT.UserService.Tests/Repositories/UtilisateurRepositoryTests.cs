using RepairIT.UserService.Data;
using RepairIT.UserService.Repositories;
using static RepairIT.UserService.Tests.UtilisateurFabrique;

namespace RepairIT.UserService.Tests.Repositories;

public class UtilisateurRepositoryTests : IDisposable
{
    private readonly UtilisateursDbContext contexte = ContexteEnMemoire();
    private readonly UtilisateurRepository repository;

    public UtilisateurRepositoryTests()
    {
        repository = new UtilisateurRepository(contexte);
        contexte.Utilisateurs.AddRange(
            Utilisateur(id: 1, nom: "Petit", email: "petit@repairit.fr"),
            Utilisateur(id: 2, nom: "Bernard", email: "bernard@repairit.fr"));
        contexte.SaveChanges();
        contexte.ChangeTracker.Clear();
    }

    [Fact]
    public async Task ListerAsync_TrieParNom()
    {
        var utilisateurs = await repository.ListerAsync();

        Assert.Equal(["Bernard", "Petit"], utilisateurs.Select(u => u.Nom));
    }

    [Theory]
    [InlineData("petit@repairit.fr", true)]
    [InlineData("inconnu@repairit.fr", false)]
    public async Task EmailExisteAsync_DetecteLesEmailsEnregistres(string email, bool attendu)
    {
        Assert.Equal(attendu, await repository.EmailExisteAsync(email));
    }

    [Fact]
    public async Task AjouterAsync_PersisteLUtilisateur()
    {
        var ajoute = await repository.AjouterAsync(Utilisateur(id: 0, email: "nouveau@repairit.fr"));

        Assert.True(ajoute.Id > 0);
        Assert.NotNull(await repository.TrouverParIdAsync(ajoute.Id));
    }

    [Fact]
    public async Task MettreAJourAsync_PersisteLesModifications()
    {
        var utilisateur = (await repository.TrouverParIdAsync(1))!;
        utilisateur.EstActif = false;

        await repository.MettreAJourAsync(utilisateur);
        contexte.ChangeTracker.Clear();

        Assert.False((await repository.TrouverParIdAsync(1))!.EstActif);
    }

    public void Dispose() => contexte.Dispose();
}

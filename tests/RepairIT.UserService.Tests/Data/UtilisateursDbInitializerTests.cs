using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using RepairIT.Shared.Configuration;
using RepairIT.Shared.Enums;
using RepairIT.UserService.Data;
using static RepairIT.UserService.Tests.UtilisateurFabrique;

namespace RepairIT.UserService.Tests.Data;

public class UtilisateursDbInitializerTests
{
    private static UtilisateursDbInitializer Initialiseur(UtilisateursDbContext contexte, bool creer = true, bool demo = true) =>
        new(contexte,
            Options.Create(new BaseDeDonneesOptions { CreerAuDemarrage = creer, InjecterDonneesDeDemo = demo }),
            NullLogger<UtilisateursDbInitializer>.Instance);

    [Fact]
    public async Task InitialiserAsync_BaseVide_InsereUnAdministrateurEtQuatreGestionnaires()
    {
        using var contexte = ContexteEnMemoire();

        await Initialiseur(contexte).InitialiserAsync();

        Assert.Equal(1, contexte.Utilisateurs.Count(u => u.Role == RoleUtilisateur.Administrateur));
        Assert.Equal(4, contexte.Utilisateurs.Count(u => u.Role == RoleUtilisateur.Gestionnaire));
    }

    [Fact]
    public async Task InitialiserAsync_BaseDejaRemplie_NeDupliquePas()
    {
        using var contexte = ContexteEnMemoire();
        contexte.Utilisateurs.Add(Utilisateur());
        await contexte.SaveChangesAsync();

        await Initialiseur(contexte).InitialiserAsync();

        Assert.Equal(1, contexte.Utilisateurs.Count());
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task InitialiserAsync_DesactiveParConfiguration_NInsereRien(bool creer, bool demo)
    {
        using var contexte = ContexteEnMemoire();

        await Initialiseur(contexte, creer, demo).InitialiserAsync();

        Assert.Empty(contexte.Utilisateurs);
    }
}

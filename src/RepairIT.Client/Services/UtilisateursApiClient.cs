using System.Net.Http.Json;
using RepairIT.Shared.Dtos.Utilisateurs;
using RepairIT.Shared.Enums;

namespace RepairIT.Client.Services;

/// <summary>Client HTTP typé du microservice Utilisateurs (injecté via HttpClientFactory).</summary>
public class UtilisateursApiClient(HttpClient http)
{
    private const string Route = "api/utilisateurs";

    public async Task<IReadOnlyList<UtilisateurDto>> ListerAsync() =>
        await http.GetFromJsonAsync<List<UtilisateurDto>>(Route) ?? [];

    public async Task CreerAsync(UtilisateurCreationDto saisie)
    {
        var reponse = await http.PostAsJsonAsync(Route, saisie);
        await ApiException.VerifierSuccesAsync(reponse);
    }

    public async Task AttribuerRoleAsync(int id, RoleUtilisateur role)
    {
        var reponse = await http.PutAsJsonAsync($"{Route}/{id}/role", new ChangementRoleDto(role));
        await ApiException.VerifierSuccesAsync(reponse);
    }

    public async Task ChangerActivationAsync(int id, bool estActif)
    {
        var action = estActif ? "activer" : "desactiver";
        var reponse = await http.PutAsync($"{Route}/{id}/{action}", content: null);
        await ApiException.VerifierSuccesAsync(reponse);
    }

    public Task<byte[]> ExporterCsvAsync() => http.GetByteArrayAsync($"{Route}/export");
}

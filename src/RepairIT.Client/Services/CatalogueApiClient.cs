using System.Net.Http.Json;
using RepairIT.Shared.Dtos.Ordinateurs;

namespace RepairIT.Client.Services;

/// <summary>Client HTTP typé du microservice Catalogue (injecté via HttpClientFactory).</summary>
public class CatalogueApiClient(HttpClient http)
{
    private const string Route = "api/ordinateurs";

    public async Task<IReadOnlyList<OrdinateurDto>> ListerAsync(string? recherche = null)
    {
        var url = string.IsNullOrWhiteSpace(recherche) ? Route : $"{Route}?recherche={Uri.EscapeDataString(recherche)}";
        return await http.GetFromJsonAsync<List<OrdinateurDto>>(url) ?? [];
    }

    public async Task<EtatStockDto?> ConsulterStockAsync() =>
        await http.GetFromJsonAsync<EtatStockDto>($"{Route}/stock");

    public async Task CreerAsync(OrdinateurSaisieDto saisie)
    {
        var reponse = await http.PostAsJsonAsync(Route, saisie);
        await ApiException.VerifierSuccesAsync(reponse);
    }

    public async Task ModifierAsync(int id, OrdinateurSaisieDto saisie)
    {
        var reponse = await http.PutAsJsonAsync($"{Route}/{id}", saisie);
        await ApiException.VerifierSuccesAsync(reponse);
    }

    public async Task SupprimerAsync(int id)
    {
        var reponse = await http.DeleteAsync($"{Route}/{id}");
        await ApiException.VerifierSuccesAsync(reponse);
    }

    public Task<byte[]> ExporterCsvAsync() => http.GetByteArrayAsync($"{Route}/export");
}

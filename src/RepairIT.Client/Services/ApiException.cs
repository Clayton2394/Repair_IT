using System.Net.Http.Json;

namespace RepairIT.Client.Services;

/// <summary>Erreur renvoyée par une API, avec un message lisible par l'utilisateur.</summary>
public class ApiException(string message) : Exception(message)
{
    /// <summary>Lève une <see cref="ApiException"/> si la réponse n'est pas un succès.</summary>
    public static async Task VerifierSuccesAsync(HttpResponseMessage reponse)
    {
        if (reponse.IsSuccessStatusCode)
        {
            return;
        }

        var probleme = await LireProblemeAsync(reponse);
        throw new ApiException(probleme?.Detail ?? probleme?.Title ?? $"Erreur du serveur ({(int)reponse.StatusCode}).");
    }

    private static async Task<DetailProbleme?> LireProblemeAsync(HttpResponseMessage reponse)
    {
        try
        {
            return await reponse.Content.ReadFromJsonAsync<DetailProbleme>();
        }
        catch (Exception)
        {
            return null;
        }
    }

    private record DetailProbleme(string? Title, string? Detail);
}

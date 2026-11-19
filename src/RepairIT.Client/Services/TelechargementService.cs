using Microsoft.JSInterop;

namespace RepairIT.Client.Services;

/// <summary>Déclenche le téléchargement d'un fichier côté navigateur (interop JavaScript).</summary>
public class TelechargementService(IJSRuntime js)
{
    public async Task TelechargerAsync(string nomFichier, byte[] contenu, string typeMime = "text/csv")
    {
        using var flux = new MemoryStream(contenu);
        using var reference = new DotNetStreamReference(flux);
        await js.InvokeVoidAsync("repairIT.telechargerFichier", nomFichier, typeMime, reference);
    }
}

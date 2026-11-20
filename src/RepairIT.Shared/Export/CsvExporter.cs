using System.Globalization;
using System.Text;

namespace RepairIT.Shared.Export;

/// <summary>
/// Génère un fichier CSV (séparateur « ; », compatible Excel FR) à partir d'une liste d'objets.
/// Chaque colonne est décrite par un en-tête et une fonction d'extraction de valeur.
/// </summary>
public static class CsvExporter
{
    private const char Separateur = ';';

    public static string Exporter<T>(IEnumerable<T> elements, IReadOnlyList<CsvColonne<T>> colonnes)
    {
        var csv = new StringBuilder();
        csv.AppendLine(string.Join(Separateur, colonnes.Select(c => Echapper(c.EnTete))));

        foreach (var element in elements)
        {
            var valeurs = colonnes.Select(c => Echapper(Formater(c.Valeur(element))));
            csv.AppendLine(string.Join(Separateur, valeurs));
        }

        return csv.ToString();
    }

    /// <summary>Encode le CSV en UTF-8 avec BOM pour qu'Excel affiche correctement les accents.</summary>
    public static byte[] EncoderPourExcel(string csv) =>
        [.. Encoding.UTF8.GetPreamble(), .. Encoding.UTF8.GetBytes(csv)];

    private static string Formater(object? valeur) => valeur switch
    {
        null => string.Empty,
        DateTime date => date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
        decimal montant => montant.ToString("0.00", CultureInfo.GetCultureInfo("fr-FR")),
        IFormattable formattable => formattable.ToString(null, CultureInfo.GetCultureInfo("fr-FR")),
        _ => valeur.ToString() ?? string.Empty
    };

    private static string Echapper(string valeur)
    {
        var doitEtreEntreGuillemets = valeur.IndexOfAny([Separateur, '"', '\n', '\r']) >= 0;
        return doitEtreEntreGuillemets ? $"\"{valeur.Replace("\"", "\"\"")}\"" : valeur;
    }
}

/// <summary>Décrit une colonne CSV : son en-tête et comment lire sa valeur.</summary>
public record CsvColonne<T>(string EnTete, Func<T, object?> Valeur);

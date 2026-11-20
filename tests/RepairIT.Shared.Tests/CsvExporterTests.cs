using System.Text;
using RepairIT.Shared.Export;

namespace RepairIT.Shared.Tests;

public class CsvExporterTests
{
    private record Ligne(string Nom, decimal Prix, DateTime Date, int? Quantite);

    private static readonly IReadOnlyList<CsvColonne<Ligne>> Colonnes =
    [
        new("Nom", l => l.Nom),
        new("Prix", l => l.Prix),
        new("Date", l => l.Date),
        new("Quantité", l => l.Quantite)
    ];

    [Fact]
    public void Exporter_EcritLEnTetePuisUneLigneParElement()
    {
        var lignes = new[]
        {
            new Ligne("Dell", 1499.5m, new DateTime(2026, 11, 20, 14, 30, 0), 3),
            new Ligne("HP", 549m, new DateTime(2026, 1, 2), null)
        };

        var csv = CsvExporter.Exporter(lignes, Colonnes);

        var resultat = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(["Nom;Prix;Date;Quantité", "Dell;1499,50;2026-11-20 14:30;3", "HP;549,00;2026-01-02 00:00;"], resultat);
    }

    [Theory]
    [InlineData("Avec;point-virgule", "\"Avec;point-virgule\"")]
    [InlineData("Avec \"guillemets\"", "\"Avec \"\"guillemets\"\"\"")]
    [InlineData("Simple", "Simple")]
    public void Exporter_EchappeLesValeursSpeciales(string valeur, string attendu)
    {
        var csv = CsvExporter.Exporter([new Ligne(valeur, 0, DateTime.MinValue, 0)], Colonnes);

        var ligneDonnees = csv.Split(Environment.NewLine)[1];
        Assert.StartsWith(attendu + ";", ligneDonnees);
    }

    [Fact]
    public void Exporter_SansElement_RenvoieSeulementLEnTete()
    {
        var csv = CsvExporter.Exporter(Array.Empty<Ligne>(), Colonnes);

        Assert.Equal("Nom;Prix;Date;Quantité" + Environment.NewLine, csv);
    }

    [Fact]
    public void EncoderPourExcel_AjouteLeBomUtf8()
    {
        var octets = CsvExporter.EncoderPourExcel("é");

        Assert.Equal(Encoding.UTF8.GetPreamble(), octets.Take(3));
        Assert.Equal("é", Encoding.UTF8.GetString(octets.Skip(3).ToArray()));
    }
}

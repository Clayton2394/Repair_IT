namespace RepairIT.CatalogService.Configuration;

/// <summary>Paramètres métier du catalogue (section « Catalogue » de appsettings.json).</summary>
public class CatalogueOptions
{
    public const string Section = "Catalogue";

    /// <summary>À partir de cette quantité (incluse), une référence est signalée en stock faible.</summary>
    public int SeuilStockFaible { get; set; } = 3;
}

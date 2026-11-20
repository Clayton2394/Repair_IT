using RepairIT.Shared.Enums;

namespace RepairIT.CatalogService.Models;

/// <summary>Entité persistée représentant un ordinateur du catalogue.</summary>
public class Ordinateur
{
    public int Id { get; set; }
    public string Marque { get; set; } = string.Empty;
    public string Modele { get; set; } = string.Empty;
    public string Processeur { get; set; } = string.Empty;
    public int MemoireRamGo { get; set; }
    public int StockageGo { get; set; }
    public decimal Prix { get; set; }
    public int QuantiteEnStock { get; set; }
    public EtatOrdinateur Etat { get; set; }
    public string? Description { get; set; }
    public DateTime DateAjout { get; set; }
}

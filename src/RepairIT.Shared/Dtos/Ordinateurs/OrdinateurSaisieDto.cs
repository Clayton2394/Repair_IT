using System.ComponentModel.DataAnnotations;
using RepairIT.Shared.Enums;

namespace RepairIT.Shared.Dtos.Ordinateurs;

/// <summary>Données saisies pour créer ou modifier un ordinateur.</summary>
public class OrdinateurSaisieDto
{
    [Required(ErrorMessage = "La marque est obligatoire.")]
    [StringLength(50)]
    public string Marque { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le modèle est obligatoire.")]
    [StringLength(100)]
    public string Modele { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le processeur est obligatoire.")]
    [StringLength(100)]
    public string Processeur { get; set; } = string.Empty;

    [Range(1, 1024, ErrorMessage = "La RAM doit être comprise entre 1 et 1024 Go.")]
    public int MemoireRamGo { get; set; } = 8;

    [Range(16, 16384, ErrorMessage = "Le stockage doit être compris entre 16 et 16384 Go.")]
    public int StockageGo { get; set; } = 256;

    [Range(0, 100000, ErrorMessage = "Le prix doit être positif.")]
    public decimal Prix { get; set; }

    [Range(0, 10000, ErrorMessage = "La quantité doit être positive.")]
    public int QuantiteEnStock { get; set; }

    public EtatOrdinateur Etat { get; set; } = EtatOrdinateur.Neuf;

    [StringLength(500)]
    public string? Description { get; set; }
}

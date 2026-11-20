using RepairIT.Shared.Enums;

namespace RepairIT.Shared.Dtos.Ordinateurs;

/// <summary>Représentation d'un ordinateur renvoyée par l'API catalogue.</summary>
public record OrdinateurDto(
    int Id,
    string Marque,
    string Modele,
    string Processeur,
    int MemoireRamGo,
    int StockageGo,
    decimal Prix,
    int QuantiteEnStock,
    EtatOrdinateur Etat,
    string? Description,
    DateTime DateAjout);

namespace RepairIT.Shared.Dtos.Ordinateurs;

/// <summary>Synthèse du stock du catalogue.</summary>
public record EtatStockDto(
    int NombreDeReferences,
    int NombreTotalUnites,
    decimal ValeurTotaleStock,
    int SeuilStockFaible,
    IReadOnlyList<OrdinateurDto> ReferencesEnStockFaible);

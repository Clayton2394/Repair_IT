using System.Text.Json.Serialization;

namespace RepairIT.Shared.Enums;

/// <summary>État commercial d'un ordinateur du catalogue.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EtatOrdinateur
{
    Neuf,
    Reconditionne,
    Occasion
}

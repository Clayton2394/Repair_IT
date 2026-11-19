using System.Text.Json.Serialization;

namespace RepairIT.Shared.Enums;

/// <summary>Rôles applicatifs (seront mappés sur les rôles Keycloak en V5).</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoleUtilisateur
{
    Gestionnaire,
    Administrateur
}

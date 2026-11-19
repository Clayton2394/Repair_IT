namespace RepairIT.UserService.Services;

/// <summary>Levée quand on tente de créer un utilisateur avec un email déjà enregistré.</summary>
public class EmailDejaUtiliseException(string email)
    : Exception($"L'email « {email} » est déjà utilisé par un autre utilisateur.");

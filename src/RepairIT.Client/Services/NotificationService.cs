namespace RepairIT.Client.Services;

public enum TypeNotification
{
    Succes,
    Erreur
}

public record Notification(Guid Id, string Message, TypeNotification Type);

/// <summary>
/// Diffuse des notifications (toasts) à travers l'application grâce à un événement C#.
/// Les pages appellent <see cref="Succes"/> / <see cref="Erreur"/>, le composant Toasts s'abonne.
/// </summary>
public class NotificationService
{
    public event Action<Notification>? NotificationEmise;

    public void Succes(string message) => Emettre(message, TypeNotification.Succes);

    public void Erreur(string message) => Emettre(message, TypeNotification.Erreur);

    private void Emettre(string message, TypeNotification type) =>
        NotificationEmise?.Invoke(new Notification(Guid.NewGuid(), message, type));
}

namespace RepairIT.Shared.Configuration;

/// <summary>Paramètres d'initialisation de la base (section « BaseDeDonnees »).</summary>
public class BaseDeDonneesOptions
{
    public const string Section = "BaseDeDonnees";

    /// <summary>Crée le schéma au démarrage s'il n'existe pas.</summary>
    public bool CreerAuDemarrage { get; set; } = true;

    /// <summary>Insère un jeu de données de démonstration si la table est vide.</summary>
    public bool InjecterDonneesDeDemo { get; set; } = true;
}

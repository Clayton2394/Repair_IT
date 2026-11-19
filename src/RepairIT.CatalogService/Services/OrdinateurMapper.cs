using RepairIT.CatalogService.Models;
using RepairIT.Shared.Dtos.Ordinateurs;

namespace RepairIT.CatalogService.Services;

/// <summary>Conversions entre l'entité <see cref="Ordinateur"/> et ses DTO.</summary>
public static class OrdinateurMapper
{
    public static OrdinateurDto VersDto(this Ordinateur o) => new(
        o.Id, o.Marque, o.Modele, o.Processeur, o.MemoireRamGo, o.StockageGo,
        o.Prix, o.QuantiteEnStock, o.Etat, o.Description, o.DateAjout);

    public static Ordinateur VersEntite(this OrdinateurSaisieDto saisie, DateTime dateAjout)
    {
        var ordinateur = new Ordinateur { DateAjout = dateAjout };
        saisie.AppliquerA(ordinateur);
        return ordinateur;
    }

    public static void AppliquerA(this OrdinateurSaisieDto saisie, Ordinateur ordinateur)
    {
        ordinateur.Marque = saisie.Marque.Trim();
        ordinateur.Modele = saisie.Modele.Trim();
        ordinateur.Processeur = saisie.Processeur.Trim();
        ordinateur.MemoireRamGo = saisie.MemoireRamGo;
        ordinateur.StockageGo = saisie.StockageGo;
        ordinateur.Prix = saisie.Prix;
        ordinateur.QuantiteEnStock = saisie.QuantiteEnStock;
        ordinateur.Etat = saisie.Etat;
        ordinateur.Description = string.IsNullOrWhiteSpace(saisie.Description) ? null : saisie.Description.Trim();
    }
}

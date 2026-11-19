using Microsoft.AspNetCore.Mvc;
using RepairIT.CatalogService.Services;
using RepairIT.Shared.Dtos.Ordinateurs;
using RepairIT.Shared.Export;

namespace RepairIT.CatalogService.Controllers;

[ApiController]
[Route("api/ordinateurs")]
public class OrdinateursController(IOrdinateurService service) : ControllerBase
{
    /// <summary>Liste le catalogue, éventuellement filtré par marque, modèle ou processeur.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrdinateurDto>>> Lister([FromQuery] string? recherche) =>
        Ok(await service.ListerAsync(recherche));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrdinateurDto>> Obtenir(int id)
    {
        var ordinateur = await service.ObtenirAsync(id);
        return ordinateur is null ? NotFound() : Ok(ordinateur);
    }

    [HttpPost]
    public async Task<ActionResult<OrdinateurDto>> Creer(OrdinateurSaisieDto saisie)
    {
        var ordinateur = await service.CreerAsync(saisie);
        return CreatedAtAction(nameof(Obtenir), new { id = ordinateur.Id }, ordinateur);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<OrdinateurDto>> Modifier(int id, OrdinateurSaisieDto saisie)
    {
        var ordinateur = await service.ModifierAsync(id, saisie);
        return ordinateur is null ? NotFound() : Ok(ordinateur);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Supprimer(int id) =>
        await service.SupprimerAsync(id) ? NoContent() : NotFound();

    [HttpGet("stock")]
    public async Task<ActionResult<EtatStockDto>> ConsulterStock() =>
        Ok(await service.ConsulterStockAsync());

    [HttpGet("export")]
    public async Task<IActionResult> Exporter()
    {
        var csv = await service.ExporterCsvAsync();
        return File(CsvExporter.EncoderPourExcel(csv), "text/csv", "catalogue.csv");
    }
}

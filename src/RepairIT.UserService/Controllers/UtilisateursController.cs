using Microsoft.AspNetCore.Mvc;
using RepairIT.Shared.Dtos.Utilisateurs;
using RepairIT.Shared.Export;
using RepairIT.UserService.Services;

namespace RepairIT.UserService.Controllers;

[ApiController]
[Route("api/utilisateurs")]
public class UtilisateursController(IUtilisateurService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UtilisateurDto>>> Lister() =>
        Ok(await service.ListerAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UtilisateurDto>> Obtenir(int id)
    {
        var utilisateur = await service.ObtenirAsync(id);
        return utilisateur is null ? NotFound() : Ok(utilisateur);
    }

    [HttpPost]
    public async Task<ActionResult<UtilisateurDto>> Creer(UtilisateurCreationDto saisie)
    {
        try
        {
            var utilisateur = await service.CreerAsync(saisie);
            return CreatedAtAction(nameof(Obtenir), new { id = utilisateur.Id }, utilisateur);
        }
        catch (EmailDejaUtiliseException exception)
        {
            return Conflict(new ProblemDetails { Title = "Email déjà utilisé", Detail = exception.Message });
        }
    }

    [HttpPut("{id:int}/role")]
    public async Task<ActionResult<UtilisateurDto>> AttribuerRole(int id, ChangementRoleDto changement)
    {
        var utilisateur = await service.AttribuerRoleAsync(id, changement.Role);
        return utilisateur is null ? NotFound() : Ok(utilisateur);
    }

    [HttpPut("{id:int}/activer")]
    public Task<ActionResult<UtilisateurDto>> Activer(int id) => ChangerActivation(id, estActif: true);

    [HttpPut("{id:int}/desactiver")]
    public Task<ActionResult<UtilisateurDto>> Desactiver(int id) => ChangerActivation(id, estActif: false);

    [HttpGet("export")]
    public async Task<IActionResult> Exporter()
    {
        var csv = await service.ExporterCsvAsync();
        return File(CsvExporter.EncoderPourExcel(csv), "text/csv", "utilisateurs.csv");
    }

    private async Task<ActionResult<UtilisateurDto>> ChangerActivation(int id, bool estActif)
    {
        var utilisateur = await service.ChangerActivationAsync(id, estActif);
        return utilisateur is null ? NotFound() : Ok(utilisateur);
    }
}

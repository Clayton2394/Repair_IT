using Microsoft.AspNetCore.Mvc;

namespace RepairIT.AuthentificationService.Controllers;

/// <summary>Point de contrôle du service (squelette V1, fonctionnalités prévues dans une version ultérieure).</summary>
[ApiController]
[Route("api/statut")]
public class StatutController(IConfiguration configuration) : ControllerBase
{
    [HttpGet]
    public IActionResult Obtenir() => Ok(new
    {
        Service = "RepairIT.AuthentificationService",
        Statut = "En ligne",
        Version = configuration["Service:Version"]
    });
}

using backend.Dtos.request;
using backend.Dtos.response;
using backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static backend.Enum.ERol;

namespace backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class NutricionistaController : ControllerBase
{
    private readonly INutricionistaService _nutricionistaService;

    public NutricionistaController(INutricionistaService nutricionistaService)
    {
        _nutricionistaService = nutricionistaService;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<NutricionistaResponseDTO>> Registrar([FromBody] RegistroNutricionistaDTO dto)
    {
        var resultado = await _nutricionistaService.RegistrarNutricionista(dto);

        return Created(string.Empty, resultado);
    }

    [Authorize(Roles = nameof(Nutricionista))]
    [HttpGet("me")]
    public async Task<ActionResult<NutricionistaResponseDTO>> ObtenerMiPerfil()
    {
        var resultado = await _nutricionistaService.ObtenerMiPerfil();
        return Ok(resultado);
    }

    [Authorize(Roles = nameof(Nutricionista))]
    [HttpPut("me")]
    public async Task<ActionResult<NutricionistaResponseDTO>> ModificarMiPerfil(
        [FromBody] NutricionistaRequestDTO dto)
    {
        var resultado = await _nutricionistaService.ModificarMiPerfil(dto);
        return Ok(resultado);
    }

    [HttpPost("subir-pdf")]
    public async Task<ActionResult<ArchivoResponseDTO>> SubirPdf([FromForm] SubirPdfRequestDTO dto)
    {
        var result = await _nutricionistaService.SubirPdf(dto);
        return Ok(result);
    }

    [HttpGet("mis-archivos")]
    public async Task<ActionResult<List<ArchivoResponseDTO>>> GetMisArchivos()
    {
        var result = await _nutricionistaService.ObtenerMisArchivos();
        return Ok(result);
    }

    [HttpDelete("archivo/{id}")]
    public async Task<IActionResult> EliminarArchivo(int id)
    {
        await _nutricionistaService.EliminarArchivo(id);
        return NoContent();
    }
}

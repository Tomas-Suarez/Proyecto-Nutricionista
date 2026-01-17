using backend.Dtos.request;
using backend.Dtos.response;
using backend.Service;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost]
    public async Task<ActionResult<NutricionistaResponseDTO>> Registrar([FromBody] RegistroNutricionistaDTO dto)
    {
        var resultado = await _nutricionistaService.RegistrarNutricionista(dto);

        return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id_Nutricionista }, resultado);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NutricionistaResponseDTO>> ObtenerPorId(int id)
    {
        var resultado = await _nutricionistaService.ObtenerNutricionistaPorId(id);
        return Ok(resultado);
    }

    [HttpGet("usuario/{usuarioId}")]
    public async Task<ActionResult<NutricionistaResponseDTO>> ObtenerPorUsuarioId(int usuarioId)
    {
        var resultado = await _nutricionistaService.ObtenerPorUsuarioId(usuarioId);
        return Ok(resultado);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<NutricionistaResponseDTO>> Actualizar(int id, [FromBody] NutricionistaRequestDTO dto)
    {
        var resultado = await _nutricionistaService.ModificarNutricionista(id, dto);
        return Ok(resultado);
    }
}

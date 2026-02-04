using backend.Dtos.Common;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static backend.Enum.ERol;

namespace backend.Controller;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PesajeController : ControllerBase
{

    private readonly IPesajeService _pesajeService;

    public PesajeController(IPesajeService pesajeService)
    {
        _pesajeService = pesajeService;
    }

    [Authorize(Roles = nameof(Nutricionista))]
    [HttpPost]
    public async Task<ActionResult<PesajeResponseDTO>> Registrar([FromBody] PesajeRequestDTO dto)
    {
        var resultado = await _pesajeService.RegistrarPesaje(dto);

        return CreatedAtAction(nameof(ObtenerPorId), new { idPesaje = resultado.Id_Pesaje }, resultado);
    }

    [Authorize(Roles = $"{nameof(Nutricionista)},{nameof(Paciente)}")]
    [HttpGet("{idPesaje}")]
    public async Task<ActionResult<PesajeResponseDTO>> ObtenerPorId(int idPesaje)
    {
        var pesaje = await _pesajeService.ObtenerPesajePorId(idPesaje);
        return Ok(pesaje);
    }

    [Authorize(Roles = nameof(Paciente))]
    [HttpGet("me")]
    public async Task<ActionResult<PagedResponseDTO<PesajeResponseDTO>>> ObtenerMiHistorial(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        var historial = await _pesajeService.ObtenerMiHistorial(page, size);
        return Ok(historial);
    }

    [Authorize(Roles = nameof(Nutricionista))]
    [HttpDelete("{idPesaje}")]
    public async Task<IActionResult> Eliminar(int idPesaje)
    {
        await _pesajeService.EliminarPesaje(idPesaje);
        return NoContent();
    }
}

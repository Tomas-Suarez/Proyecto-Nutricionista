using backend.Dtos.Common;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Service;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class PesajeController : ControllerBase
{

    private readonly IPesajeService _pesajeService;

    public PesajeController(IPesajeService pesajeService)
    {
        _pesajeService = pesajeService;
    }

    [HttpPost]
    public async Task<ActionResult<PesajeResponseDTO>> Registrar([FromBody] PesajeRequestDTO dto)
    {
        var resultado = await _pesajeService.RegistrarPesaje(dto);

        return CreatedAtAction(nameof(ObtenerPorId), new { idPesaje = resultado.Id_Pesaje }, resultado);
    }

    [HttpGet("{idPesaje}")]
    public async Task<ActionResult<PesajeResponseDTO>> ObtenerPorId(int idPesaje)
    {
        var pesaje = await _pesajeService.ObtenerPesajePorId(idPesaje);
        return Ok(pesaje);
    }

    [HttpGet("paciente/{idPaciente}")]
    public async Task<ActionResult<PagedResponseDTO<PesajeResponseDTO>>> ObtenerHistorial(
        int idPaciente,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        var historial = await _pesajeService.ObtenerHistorialPesaje(idPaciente, page, size);
        return Ok(historial);
    }

    [HttpDelete("{idPesaje}")]
    public async Task<IActionResult> Eliminar(int idPesaje)
    {
        await _pesajeService.EliminarPesaje(idPesaje);
        return NoContent();
    }
}

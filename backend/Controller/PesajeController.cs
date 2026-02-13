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

    [Authorize(Roles = nameof(Nutricionista))]
    [HttpGet("{idPesaje}")]
    public async Task<ActionResult<PesajeResponseDTO>> ObtenerPorId(int idPesaje)
    {
        var pesaje = await _pesajeService.ObtenerPesajePorId(idPesaje);
        return Ok(pesaje);
    }

    [Authorize(Roles = nameof(Nutricionista))]
    [HttpDelete("{idPesaje}")]
    public async Task<IActionResult> Eliminar(int idPesaje)
    {
        await _pesajeService.EliminarPesaje(idPesaje);
        return NoContent();
    }

    [Authorize(Roles = nameof(Nutricionista))]
    [HttpGet("historial/{idPaciente}")]
    public async Task<ActionResult<PagedResponseDTO<PesajeResponseDTO>>> ObtenerHistorialPaciente(
        int idPaciente,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        var historial = await _pesajeService.ObtenerHistorialPorPaciente(idPaciente, page, size);
        return Ok(historial);
    }

    [AllowAnonymous]
    [HttpPost("publico/historial")]
    public async Task<ActionResult<PagedResponseDTO<PesajeResponseDTO>>> ObtenerHistorialPublico(
        [FromBody] LoginPacienteDTO credenciales,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        var historial = await _pesajeService.ObtenerHistorialPublico(
            credenciales.Token, 
            credenciales.Codigo, 
            page, 
            size
        );
        return Ok(historial);
    }
}
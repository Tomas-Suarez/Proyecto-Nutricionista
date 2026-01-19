using backend.Dtos.Common;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Enum;
using backend.Service;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controller;

[Route("api/[controller]")]
[ApiController]
public class PacienteController : ControllerBase
{
    private IPacienteService _pacienteService;

    public PacienteController(IPacienteService pacienteService)
    {
        _pacienteService = pacienteService;
    }

    [HttpPost]
    public async Task<ActionResult<PacienteResponseDTO>> Registrar([FromBody] RegistroPacienteDTO dto)
    {
        var resultado = await _pacienteService.RegistrarPaciente(dto);

        return CreatedAtAction( nameof(ObtenerPorId), new { idPaciente = resultado.Id_Paciente }, resultado);
    }

    [HttpGet("{idPaciente}")]
    public async Task<ActionResult<PacienteResponseDTO>> ObtenerPorId(int idPaciente)
    {
        var resultado = await _pacienteService.ObtenerPacientePorId(idPaciente);

        return Ok(resultado);
    }

    [HttpGet("usuario/{idUsuario}")]
    public async Task<ActionResult<PacienteResponseDTO>> ObtenerPorUsuarioId(int idUsuario)
    {
        var resultado = await _pacienteService.ObtenerPorUsuarioId(idUsuario);

        return Ok(resultado);
    }

    [HttpPut("{idPaciente}")]
    public async Task<ActionResult<PacienteResponseDTO>> Actualizar(int idPaciente, [FromBody] PacienteRequestDTO dto)
    {
        var resultado = await _pacienteService.ModificarPaciente(idPaciente, dto);

        return Ok(resultado);
    }

    [HttpGet("nutricionista/{idNutricionista}")]
    public async Task<ActionResult<PagedResponseDTO<PacienteResponseDTO>>> ListarPorNutricionista(
    int idNutricionista,
    [FromQuery] EEstadoPaciente? estado,
    [FromQuery] int page = 1,
    [FromQuery] int size = 10)
    {
        var resultado = await _pacienteService.ObtenerPacientesPorNutricionista(idNutricionista, page, size, estado);
        return Ok(resultado);
    }

    [HttpPatch("{idPaciente}/estado")]
    public async Task<IActionResult> ActualizarEstado(int idPaciente, [FromBody] EEstadoPaciente nuevoEstado)
    {
        await _pacienteService.CambiarEstado(idPaciente, nuevoEstado);
        return NoContent();
    }
}
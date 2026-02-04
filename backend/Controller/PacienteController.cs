using backend.Dtos.Common;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Enum;
using backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static backend.Enum.ERol;


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

    [Authorize(Roles = $"{nameof(Nutricionista)},{nameof(Admin)}")]
    [HttpPost]
    public async Task<ActionResult<PacienteResponseDTO>> Registrar([FromBody] RegistroPacienteDTO dto)
    {
        var resultado = await _pacienteService.RegistrarPaciente(dto);

        return Created(string.Empty, resultado);
    }

    [Authorize(Roles = nameof(Paciente))]
    [HttpGet("me")]
    public async Task<ActionResult<PacienteResponseDTO>> ObtenerMiPerfil()
    {
        var resultado = await _pacienteService.ObtenerMiPerfil();
        return Ok(resultado);
    }

    [Authorize(Roles = nameof(Paciente))]
    [HttpPut("me")]
    public async Task<ActionResult<PacienteResponseDTO>> ModificarMiPerfil([FromBody] PacienteRequestDTO dto)
    {
        var resultado = await _pacienteService.ModificarMiPerfil(dto);
        return Ok(resultado);
    }

    [Authorize(Roles = $"{nameof(Nutricionista)},{nameof(Admin)}")]
    [HttpPut("{idPaciente}")]
    public async Task<ActionResult<PacienteResponseDTO>> ActualizarPaciente(int idPaciente, [FromBody] PacienteRequestDTO dto)
    {
        var resultado = await _pacienteService.ModificarPaciente(idPaciente, dto);
        return Ok(resultado);
    }

    [Authorize(Roles = $"{nameof(Nutricionista)},{nameof(Admin)}")]
    [HttpGet]
    public async Task<ActionResult<PagedResponseDTO<PacienteResponseDTO>>> ListarMisPacientes(
        [FromQuery] EEstadoPaciente? estado,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        var resultado = await _pacienteService.ObtenerPacientesPorNutricionista(
            page, size, estado);

        return Ok(resultado);
    }

    [Authorize(Roles = $"{nameof(Nutricionista)},{nameof(Admin)}")]
    [HttpPatch("{idPaciente}/estado")]
    public async Task<IActionResult> CambiarEstado(int idPaciente, [FromBody] EEstadoPaciente nuevoEstado)
    {
        await _pacienteService.CambiarEstado(idPaciente, nuevoEstado);
        return NoContent();
    }
}
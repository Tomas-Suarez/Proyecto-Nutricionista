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
public class DietaController : ControllerBase
{
    private readonly IDietaService _dietaService;

    public DietaController(IDietaService dietaService)
    {
        _dietaService = dietaService;
    }

    [Authorize(Roles = nameof(Nutricionista))]
    [HttpPost]
    public async Task<ActionResult<DietaResponseDTO>> Registrar([FromBody] DietaRequestDTO dto)
    {
        var resultado = await _dietaService.CrearDieta(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id_Dieta }, resultado);
    }

    [Authorize(Roles = nameof(Nutricionista))]
    [HttpGet("{id}")]
    public async Task<ActionResult<DietaResponseDTO>> ObtenerPorId(int id)
    {
        var resultado = await _dietaService.ObtenerPorId(id);
        return Ok(resultado);
    }

    [Authorize(Roles = nameof(Nutricionista))]
    [HttpPut("{id}")]
    public async Task<ActionResult<DietaResponseDTO>> Actualizar(int id, [FromBody] DietaRequestDTO dto)
    {
        var resultado = await _dietaService.ActualizarDieta(id, dto);
        return Ok(resultado);
    }

    [Authorize(Roles = nameof(Nutricionista))]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Eliminar(int id)
    {
        await _dietaService.EliminarDieta(id);
        return NoContent();
    }

    [Authorize(Roles = nameof(Nutricionista))]
    [HttpGet("paciente/{idPaciente}/historial")]
    public async Task<ActionResult<PagedResponseDTO<DietaResponseDTO>>> ObtenerHistorial(
        int idPaciente,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        var resultado = await _dietaService.ObtenerHistorialPaciente(idPaciente, page, size);
        return Ok(resultado);
    }

    [Authorize(Roles = nameof(Nutricionista))]
    [HttpGet("paciente/{idPaciente}/activa")]
    public async Task<ActionResult<DietaResponseDTO>> ObtenerActiva(int idPaciente)
    {
        var resultado = await _dietaService.ObtenerDietaActiva(idPaciente);
        return Ok(resultado);
    }

    [Authorize(Roles = nameof(Nutricionista))]
    [HttpPatch("{id}/activar")]
    public async Task<IActionResult> ActivarDieta(int id)
    {
        await _dietaService.ActivarDieta(id);
        return Ok(new { message = "Dieta activada correctamente." });
    }

    [AllowAnonymous]
    [HttpPost("publico/activa")]
    public async Task<ActionResult<DietaResponseDTO>> ObtenerDietaActivaPublica([FromBody] LoginPacienteDTO credenciales)
    {
        var resultado = await _dietaService.ObtenerDietaActualPublica(credenciales.Token, credenciales.Codigo);

        if (resultado == null) return NotFound("El paciente no tiene una dieta activa asignada.");

        return Ok(resultado);
    }
}
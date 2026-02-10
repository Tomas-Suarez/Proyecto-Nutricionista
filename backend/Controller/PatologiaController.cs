using backend.Dtos.request;
using backend.Dtos.response;
using backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static backend.Enum.ERol;


namespace backend.Controller;

[Route("api/[controller]")]
[ApiController]
public class PatologiaController : ControllerBase
{
    private readonly IPatologiaService _patologiaService;

    public PatologiaController(IPatologiaService patologiaService)
    {
        _patologiaService = patologiaService;
    }

    [Authorize(Roles = $"{nameof(Nutricionista)},{nameof(Admin)}")]
    [HttpGet]
    public async Task<ActionResult<List<PatologiaResponseDTO>>> ListarTodas()
    {
        return Ok(await _patologiaService.ListarTodas());
    }

    [Authorize(Roles = nameof(Admin))]
    [HttpPost]
    public async Task<ActionResult<PatologiaResponseDTO>> Crear([FromBody] PatologiaRequestDTO dto)
    {
        var resultado = await _patologiaService.Crear(dto);
        return Created(string.Empty, resultado);
    }

    [Authorize(Roles = nameof(Admin))]
    [HttpPut("{id}")]
    public async Task<ActionResult<PatologiaResponseDTO>> Modificar(int id, [FromBody] PatologiaRequestDTO dto)
    {
        return Ok(await _patologiaService.Modificar(id, dto));
    }

    [Authorize(Roles = nameof(Admin))]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _patologiaService.Eliminar(id);
        return NoContent();
    }
}

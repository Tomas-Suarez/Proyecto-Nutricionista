using backend.Dtos.request;
using backend.Dtos.response;
using backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static backend.Enum.ERol;


namespace backend.Controller;

[Route("api/[controller]")]
[ApiController]
public class ObjetivoController : ControllerBase
{
    private readonly IObjetivoService _objetivoService;

    public ObjetivoController(IObjetivoService objetivoService)
    {
        _objetivoService = objetivoService;
    }

    [Authorize(Roles = $"{nameof(Nutricionista)},{nameof(Admin)}")]
    [HttpGet]
    public async Task<ActionResult<List<ObjetivoResponseDTO>>> ListarTodos()
    {
        return Ok(await _objetivoService.ListarTodos());
    }

    [Authorize(Roles = nameof(Admin))]
    [HttpPost]
    public async Task<ActionResult<ObjetivoResponseDTO>> Crear([FromBody] ObjetivoRequestDTO dto)
    {
        var resultado = await _objetivoService.Crear(dto);
        return Created(string.Empty, resultado);
    }

    [Authorize(Roles = nameof(Admin))]
    [HttpPut("{id}")]
    public async Task<ActionResult<ObjetivoResponseDTO>> Modificar(int id, [FromBody] ObjetivoRequestDTO dto)
    {
        return Ok(await _objetivoService.Modificar(id, dto));
    }

    [Authorize(Roles = nameof(Admin))]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _objetivoService.Eliminar(id);
        return NoContent();
    }
}

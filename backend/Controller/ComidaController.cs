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
public class ComidaController : ControllerBase
{

    private readonly IComidaService _comidaService;

    public ComidaController(IComidaService comidaService)
    {
        _comidaService = comidaService;
    }

    [Authorize(Roles = nameof(Admin))]
    [HttpPost]
    public async Task<ActionResult<ComidaResponseDTO>> Crear([FromBody] ComidaRequestDTO dto)
    {
        var resultado = await _comidaService.RegistrarComida(dto);

        return CreatedAtAction(nameof(ObtenerPorId), new { idComida = resultado.Id_Comida }, resultado);
    }

    [HttpGet("{idComida}")]
    public async Task<ActionResult<ComidaResponseDTO>> ObtenerPorId(int idComida)
    {
        var comida = await _comidaService.ObtenerPorId(idComida);
        return Ok(comida);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponseDTO<ComidaResponseDTO>>> ListaComidas(
        [FromQuery] int? idCategoria,
        [FromQuery] string? nombre, 
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        var lista = await _comidaService.ObtenerTodas(page, size, idCategoria, nombre);
        return Ok(lista);
    }

    [Authorize(Roles = nameof(Admin))]
    [HttpDelete("{idComida}")]
    public async Task<ActionResult> EliminarComida(int idComida)
    {
        await _comidaService.EliminarComida(idComida);
        return NoContent();
    }

    [Authorize(Roles = nameof(Admin))]
    [HttpPut("{idComida}")]
    public async Task<ActionResult<ComidaResponseDTO>> ActualizarComida(int idComida, ComidaRequestDTO dto)
    {
        var comida = await _comidaService.ActualizarComida(idComida, dto);
        return Ok(comida);
    }
}

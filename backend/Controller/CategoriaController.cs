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
public class CategoriaController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;

    public CategoriaController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    [Authorize(Roles = nameof(Admin))]
    [HttpPost]
    public async Task<ActionResult<CategoriaResponseDTO>> Crear([FromBody] CategoriaRequestDTO dto)
    {
        var resultado = await _categoriaService.CrearCategoria(dto);

        return CreatedAtAction(nameof(ObtenerPorId), new { idCategoria = resultado.Id_Categoria }, resultado);
    }

    [HttpGet("{idCategoria}")]
    public async Task<ActionResult<CategoriaResponseDTO>> ObtenerPorId(int idCategoria)
    {
        var categoria = await _categoriaService.ObtenerPorId(idCategoria);
        return Ok(categoria);
    }

    [Authorize(Roles = $"{nameof(Admin)},{nameof(Nutricionista)}")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaResponseDTO>>> ObtenerTodas()
    {
        var categorias = await _categoriaService.ObtenerTodasCategorias();
        return Ok(categorias);
    }

    [Authorize(Roles = nameof(Admin))]
    [HttpDelete("{idCategoria}")]
    public async Task<IActionResult> Eliminar(int idCategoria)
    {
        await _categoriaService.EliminarCategoria(idCategoria);
        return NoContent();
    }

    [Authorize(Roles = nameof(Admin))]
    [HttpPut("{idCategoria}")]
    public async Task<ActionResult<CategoriaResponseDTO>> Modificar(int idCategoria, [FromBody] CategoriaRequestDTO dto)
    {
        var resultado = await _categoriaService.ModificarCategoria(idCategoria, dto);
        return Ok(resultado);
    }
}

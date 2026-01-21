using backend.Dtos.request;
using backend.Dtos.response;
using backend.Service;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;

    public CategoriaController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaResponseDTO>>> ObtenerTodas()
    {
        var categorias = await _categoriaService.ObtenerTodasCategorias();
        return Ok(categorias);
    }

    [HttpDelete("{idCategoria}")]
    public async Task<IActionResult> Eliminar(int idCategoria)
    {
        await _categoriaService.EliminarCategoria(idCategoria);
        return NoContent();
    }
}

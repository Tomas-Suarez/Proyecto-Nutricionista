using backend.Dtos.request;
using backend.Service;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] UsuarioRequestDTO dto)
        {
            try
            {
                var resultado = await _usuarioService.RegistrarUsuario(dto);

                return CreatedAtAction(nameof(Registrar), new { id = resultado.Id_Usuario }, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioRequestDTO dto)
        {
            try
            {
                var resultado = await _usuarioService.LoguearUsuario(dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { mensaje = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var resultado = await _usuarioService.ObtenerUsuarioPorId(id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        [HttpPatch("cambiar-password/{id}")]
        public async Task<IActionResult> CambiarPassword(int id, [FromBody] CambiarPasswordRequestDTO dto)
        {
            try
            {
                var resultado = await _usuarioService.CambiarPassword(id, dto);
                return Ok(new { mensaje = "Contraseña actualizada con éxito" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}

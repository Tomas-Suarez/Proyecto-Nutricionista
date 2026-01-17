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

            var resultado = await _usuarioService.RegistrarUsuario(dto);

            return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id_Usuario }, resultado);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioRequestDTO dto)
        {

            var resultado = await _usuarioService.LoguearUsuario(dto);
            return Ok(resultado);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {

            var resultado = await _usuarioService.ObtenerUsuarioPorId(id);
            return Ok(resultado);

        }

        [HttpPatch("cambiar-password/{id}")]
        public async Task<IActionResult> CambiarPassword(int id, [FromBody] CambiarPasswordRequestDTO dto)
        {

            await _usuarioService.CambiarPassword(id, dto);

            return Ok(new { mensaje = "Contraseña actualizada con éxito" });

        }
    }
}

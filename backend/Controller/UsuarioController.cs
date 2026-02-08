using backend.Dtos.request;
using backend.Exceptions;
using backend.Service;
using static backend.Constants.AuthConstants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IConfiguration _configuration;

        public UsuarioController(IUsuarioService usuarioService, IConfiguration configuration)
        {
            _usuarioService = usuarioService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] UsuarioRequestDTO dto)
        {

            var resultado = await _usuarioService.RegistrarUsuario(dto);

            return Created(string.Empty, resultado);

        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioRequestDTO dto)
        {
            var resultado = await _usuarioService.LoguearUsuario(dto);

            SetTokenCookies(resultado.Token, resultado.RefreshToken);

            return Ok(resultado.Usuario);

        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var resultado = await _usuarioService.ObtenerUsuarioPorId(id);
            return Ok(resultado);

        }

        [Authorize]
        [HttpPatch("me/password")]
        public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordRequestDTO dto)
        {

            await _usuarioService.CambiarMiPassword(dto);

            return Ok(new { mensaje = "Contraseña actualizada con éxito" });

        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies[REFRESH_TOKEN_COOKIE];

            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new SessionExpiredException("No hay token de refresco en las cookies");
            }

            var resultado = await _usuarioService.RefrescarToken(refreshToken);

            SetTokenCookies(resultado.Token, resultado.RefreshToken);

            return Ok(resultado.Usuario);
        }

        [Authorize]
        [HttpPost("avatar")]
        public async Task<IActionResult> SubirAvatar(IFormFile archivo)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id" || c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new UnauthenticatedException("Token inválido.");

            }

            int userId = int.Parse(userIdClaim.Value);

            var resultado = await _usuarioService.SubirAvatar(userId, archivo);

            return Ok(new { url = resultado.AvatarUrl });
        }

        [Authorize]
        [HttpDelete("avatar")]
        public async Task<IActionResult> BorrarAvatar()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id" || c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new UnauthenticatedException("Token inválido.");
            }

            int userId = int.Parse(userIdClaim.Value);

            var resultado = await _usuarioService.BorrarAvatar(userId);

            return Ok(new
            {
                mensaje = "Avatar eliminado correctamente",
                url = resultado.AvatarUrl
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(ACCESS_TOKEN_COOKIE);
            Response.Cookies.Delete(REFRESH_TOKEN_COOKIE);
            return Ok(new { message = "Sesión cerrada con exito." });
        }

        private void SetTokenCookies(string accessToken, string refreshToken)
        {
            var accessMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "15");
            var refreshDays = int.Parse(_configuration["Jwt:RefreshTokenExpireDays"] ?? "7");

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(accessMinutes)
            };

            Response.Cookies.Append(ACCESS_TOKEN_COOKIE, accessToken, cookieOptions);

            cookieOptions.Expires = DateTime.UtcNow.AddDays(refreshDays);
            Response.Cookies.Append(REFRESH_TOKEN_COOKIE, refreshToken, cookieOptions);
        }
    }
}

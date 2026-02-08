using backend.Data;
using backend.Dtos.request;
using backend.Dtos.response;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using backend.Models;
using backend.Exceptions;
using backend.Jwt;
using backend.Service.Common;
using Microsoft.AspNetCore.Http.HttpResults;

namespace backend.Service.imp
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _usuarioMapper;
        private readonly JwtTokenGenerator _jwtGenerator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private const string DEFAULT_AVATAR_URL = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSul0lklnuStig202SiXNvYYD_OUvmFw9KaPA&s";
        public UsuarioService(AppDbContext context, IMapper usuarioMapper, JwtTokenGenerator jwtGenerator, ICurrentUserService currentUserService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _usuarioMapper = usuarioMapper;
            _jwtGenerator = jwtGenerator;
            _currentUserService = currentUserService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<UsuarioResponseDTO> RegistrarUsuario(UsuarioRequestDTO dto)
        {
            bool existe = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email);
            if (existe)
            {
                throw new DuplicateResourceException($"El email {dto.Email} ya se encuentra registrado");
            }

            var usuarioEntity = _usuarioMapper.Map<UsuarioEntity>(dto);

            usuarioEntity.Password_Hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            usuarioEntity.Avatar_Url = DEFAULT_AVATAR_URL;

            _context.Usuarios.Add(usuarioEntity);
            await _context.SaveChangesAsync();

            return _usuarioMapper.Map<UsuarioResponseDTO>(usuarioEntity);
        }

        public async Task<LoginResponseDTO> LoguearUsuario(UsuarioRequestDTO dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == dto.Email)
                ?? throw new InvalidCredentialsException("Credenciales incorrectas");

            bool pwValida = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.Password_Hash);

            if (!pwValida)
            {
                throw new InvalidCredentialsException("Credenciales incorrectas");
            }

            var accessToken = _jwtGenerator.GenerateToken(usuario);
            var refreshToken = Guid.NewGuid().ToString();

            usuario.RefreshToken = refreshToken;
            usuario.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return new LoginResponseDTO(
                _usuarioMapper.Map<UsuarioResponseDTO>(usuario),
                accessToken,
                refreshToken
            );
        }

        public async Task<LoginResponseDTO> RefrescarToken(string refreshTokenActual)
        {
            var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshTokenActual);

            if (usuario == null || usuario.RefreshTokenExpiry < DateTime.UtcNow)
            {
                throw new SessionExpiredException();
            }

            var nuevoRefreshToken = Guid.NewGuid().ToString();
            usuario.RefreshToken = nuevoRefreshToken;
            usuario.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            var nuevoAccessToken = _jwtGenerator.GenerateToken(usuario);

            await _context.SaveChangesAsync();

            return new LoginResponseDTO(
                _usuarioMapper.Map<UsuarioResponseDTO>(usuario),
                nuevoAccessToken,
                nuevoRefreshToken
            );

        }

        public async Task<UsuarioResponseDTO> CambiarMiPassword(CambiarPasswordRequestDTO dto)
        {
            var userId = _currentUserService.GetUserId()
                ?? throw new UnauthenticatedException("Usuario no autenticado.");

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id_Usuario == userId)
                ?? throw new ResourceNotFoundException($"No se encontró el usuario con el id: {userId}");

            bool pwValida = BCrypt.Net.BCrypt.Verify(
                dto.PasswordActual,
                usuario.Password_Hash);

            if (!pwValida)
            {
                throw new InvalidCredentialsException("La contraseña actual es incorrecta.");
            }

            usuario.Password_Hash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordNueva);

            await _context.SaveChangesAsync();

            return _usuarioMapper.Map<UsuarioResponseDTO>(usuario);
        }

        public async Task<UsuarioResponseDTO> ObtenerUsuarioPorId(int idUsuario)
        {
            var idSolicitante = _currentUserService.GetUserId();

            if (!_currentUserService.IsAdmin() && idSolicitante != idUsuario)
            {
                throw new AccessDeniedException("No tienes permitido el acceso");
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id_Usuario == idUsuario)
                ?? throw new ResourceNotFoundException($"No se encontró el usuario con el id: {idUsuario}");

            return _usuarioMapper.Map<UsuarioResponseDTO>(usuario);
        }

        public async Task<UsuarioResponseDTO> SubirAvatar(int idUsuario, IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                throw new BadRequestException("No se ha enviado archivo.");
            }

            var extension = Path.GetExtension(archivo.FileName).ToLower();
            string[] permitidas = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            if (!permitidas.Contains(extension))
            {
                throw new BadRequestException("Formato no válido.");
            }

            var usuario = await _context.Usuarios.FindAsync(idUsuario)
                 ?? throw new ResourceNotFoundException($"Usuario no encontrado");

            if (usuario.Avatar_Url != DEFAULT_AVATAR_URL)
            {
                EliminarArchivoFisico(usuario.Avatar_Url!);
            }

            string carpetaNombre = Path.Combine("uploads", "avatars");
            string rutaRaiz = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string rutaFisica = Path.Combine(rutaRaiz, carpetaNombre);

            if (!Directory.Exists(rutaFisica))
            {
                Directory.CreateDirectory(rutaFisica);
            }


            string nombreArchivo = $"avatar_{idUsuario}_{Guid.NewGuid()}{extension}";
            string rutaCompleta = Path.Combine(rutaFisica, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            usuario.Avatar_Url = $"/{carpetaNombre.Replace("\\", "/")}/{nombreArchivo}";
            await _context.SaveChangesAsync();

            return _usuarioMapper.Map<UsuarioResponseDTO>(usuario);
        }

        public async Task<UsuarioResponseDTO> BorrarAvatar(int idUsuario)
        {
            var usuario = await _context.Usuarios.FindAsync(idUsuario)
                 ?? throw new ResourceNotFoundException($"Usuario no encontrado");

            if (usuario.Avatar_Url == DEFAULT_AVATAR_URL)
            {
                return _usuarioMapper.Map<UsuarioResponseDTO>(usuario);
            }

            EliminarArchivoFisico(usuario.Avatar_Url!);

            usuario.Avatar_Url = DEFAULT_AVATAR_URL;

            await _context.SaveChangesAsync();

            return _usuarioMapper.Map<UsuarioResponseDTO>(usuario);
        }

        private void EliminarArchivoFisico(string urlRelativa)
        {
            try
            {
                if (string.IsNullOrEmpty(urlRelativa) || urlRelativa == DEFAULT_AVATAR_URL)
                {
                    return;
                }

                if (!urlRelativa.StartsWith("/"))
                {
                    return;
                }

                string webRootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string pathRelativo = urlRelativa.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
                string pathCompleto = Path.Combine(webRootPath, pathRelativo);

                if (File.Exists(pathCompleto))
                {
                    File.Delete(pathCompleto);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error borrando imagen: {ex.Message}");
            }
        }
    }
}

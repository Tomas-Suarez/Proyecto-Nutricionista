using backend.Data;
using backend.Dtos.request;
using backend.Dtos.response;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using backend.Models;
using backend.Exceptions;
using backend.Jwt;
using backend.Service.Common;

namespace backend.Service.imp
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _usuarioMapper;
        private readonly JwtTokenGenerator _jwtGenerator;
        private readonly ICurrentUserService _currentUserService;
        public UsuarioService(AppDbContext context, IMapper usuarioMapper, JwtTokenGenerator jwtGenerator, ICurrentUserService currentUserService)
        {
            _context = context;
            _usuarioMapper = usuarioMapper;
            _jwtGenerator = jwtGenerator;
            _currentUserService = currentUserService;
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

            usuarioEntity.Avatar_Url = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSul0lklnuStig202SiXNvYYD_OUvmFw9KaPA&s";

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

        public async Task<UsuarioResponseDTO> CambiarPassword(int idUsuario, CambiarPasswordRequestDTO dto)
        {

            if (idUsuario != _currentUserService.GetUserId())
            {
                throw new AccessDeniedException("No puedes cambiar la contraseña de otra cuenta.");
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id_Usuario == idUsuario)
                ?? throw new ResourceNotFoundException($"No se encontró el usuario con el id: {idUsuario}");

            bool pwValida = BCrypt.Net.BCrypt.Verify(dto.PasswordActual, usuario.Password_Hash);

            if (!pwValida)
            {
                throw new InvalidCredentialsException("La contraseña actual es incorrecta.");
            }

            usuario.Password_Hash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordNueva);

            _context.Usuarios.Update(usuario);
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
    }
}

using backend.Data;
using backend.Dtos.request;
using backend.Dtos.response;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using backend.Models;
using backend.Exceptions;

namespace backend.Service.imp
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _usuarioMapper;

        public UsuarioService(AppDbContext context, IMapper usuarioMapper)
        {
            _context = context;
            _usuarioMapper = usuarioMapper;
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

        public async Task<UsuarioResponseDTO> LoguearUsuario(UsuarioRequestDTO dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == dto.Email)
                ?? throw new InvalidCredentialsException("Credenciales incorrectas");

            bool pwValida = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.Password_Hash);

            if (!pwValida)
            {
                throw new InvalidCredentialsException("Credenciales incorrectas");
            }

            return _usuarioMapper.Map<UsuarioResponseDTO>(usuario);
        }

        public async Task<UsuarioResponseDTO> CambiarPassword(int idUsuario, CambiarPasswordRequestDTO dto)
        {
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
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id_Usuario == idUsuario)
                ?? throw new ResourceNotFoundException($"No se encontró el usuario con el id: {idUsuario}");

            return _usuarioMapper.Map<UsuarioResponseDTO>(usuario);
        }
    }
}

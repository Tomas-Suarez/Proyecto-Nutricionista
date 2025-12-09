using backend.Data;
using backend.Dtos.request;
using backend.Dtos.response;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using backend.Models;

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
                throw new Exception("El email ya se encuentra registrado");
            }

            var usuarioEntity = _usuarioMapper.Map<UsuarioEntity>(dto);

            usuarioEntity.Password_Hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            usuarioEntity.Avatar_Url = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSul0lklnuStig202SiXNvYYD_OUvmFw9KaPA&s";

            _context.Usuarios.Add(usuarioEntity);
            await _context.SaveChangesAsync();

            return _usuarioMapper.Map<UsuarioResponseDTO>(usuarioEntity);
        }
    }
}

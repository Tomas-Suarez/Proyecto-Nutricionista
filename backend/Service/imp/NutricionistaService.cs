using System.Globalization;
using AutoMapper;
using backend.Data;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Enum;
using backend.Exceptions;
using backend.Models;
using backend.Service.Common;
using Microsoft.EntityFrameworkCore;

namespace backend.Service.imp;

public class NutricionistaService : INutricionistaService
{
    private readonly AppDbContext _context;
    private readonly IMapper _nutricionistaMapper;
    private readonly IUsuarioService _usuarioService;
    private readonly ICurrentUserService _currentUserService;

    public NutricionistaService(AppDbContext context, IMapper nutricionistaMapper, IUsuarioService usuarioService, ICurrentUserService currentUserService)
    {
        _context = context;
        _nutricionistaMapper = nutricionistaMapper;
        _usuarioService = usuarioService;
        _currentUserService = currentUserService;
    }

    public async Task<NutricionistaResponseDTO> RegistrarNutricionista(RegistroNutricionistaDTO dto)
    {

        if (_currentUserService.GetUserId().HasValue && !_currentUserService.IsAdmin())
        {
            throw new AccessDeniedException("Ya tienes una cuenta activa. Debes cerrar sesión para registrar un nuevo profesional.");
        }
        using var transaction = await _context.Database.BeginTransactionAsync();

        var usuarioDto = new UsuarioRequestDTO(
            dto.Email,
            dto.Password,
            ERol.Nutricionista);

        var usuarioResponse = await _usuarioService.RegistrarUsuario(usuarioDto);

        var nutricionista = _nutricionistaMapper.Map<NutricionistaEntity>(dto);
        nutricionista.Id_Usuario = usuarioResponse.Id_Usuario;

        _context.Nutricionistas.Add(nutricionista);
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();
        return _nutricionistaMapper.Map<NutricionistaResponseDTO>(nutricionista);
    }

    public async Task<NutricionistaResponseDTO> ObtenerMiPerfil()
    {
        var userId = _currentUserService.GetUserId()
            ?? throw new UnauthenticatedException("Usuario no autenticado...");

        var nutricionista = await _context.Nutricionistas
            .Include(n => n.Usuario)
            .FirstOrDefaultAsync(n => n.Id_Usuario == userId)
            ?? throw new ResourceNotFoundException("Perfil no encontrado.");

        return _nutricionistaMapper.Map<NutricionistaResponseDTO>(nutricionista);
    }

    public async Task<NutricionistaResponseDTO> ModificarMiPerfil(NutricionistaRequestDTO dto)
    {
        var userId = _currentUserService.GetUserId()
            ?? throw new UnauthenticatedException("Usuario no autenticado...");

        var nutricionista = await _context.Nutricionistas
            .FirstOrDefaultAsync(n => n.Id_Usuario == userId)
            ?? throw new ResourceNotFoundException("Perfil no encontrado.");

        _nutricionistaMapper.Map(dto, nutricionista);
        await _context.SaveChangesAsync();

        return _nutricionistaMapper.Map<NutricionistaResponseDTO>(nutricionista);
    }

}
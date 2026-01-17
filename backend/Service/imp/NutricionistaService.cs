using AutoMapper;
using backend.Data;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Enum;
using backend.Exceptions;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Service.imp;

public class NutricionistaService : INutricionistaService
{
    private readonly AppDbContext _context;
    private readonly IMapper _nutricionistaMapper;
    private readonly IUsuarioService _usuarioService;

    public NutricionistaService(AppDbContext context, IMapper nutricionistaMapper, IUsuarioService usuarioService)
    {
        _context = context;
        _nutricionistaMapper = nutricionistaMapper;
        _usuarioService = usuarioService;
    }

    public async Task<NutricionistaResponseDTO> RegistrarNutricionista(RegistroNutricionistaDTO dto)
    {
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
        return await ObtenerNutricionistaConDetalles(nutricionista.Id_Nutricionista);
    }

    public async Task<NutricionistaResponseDTO> ObtenerNutricionistaPorId(int idNutricionista)
    {
        return await ObtenerNutricionistaConDetalles(idNutricionista);
    }

    public async Task<NutricionistaResponseDTO> ObtenerPorUsuarioId(int idUsuario)
    {
        var nutricionista = await _context.Nutricionistas
            .Include(n => n.Usuario)
            .FirstOrDefaultAsync(n => n.Id_Usuario == idUsuario)
            ?? throw new ResourceNotFoundException("Nutricionista no encontrado.");

        return _nutricionistaMapper.Map<NutricionistaResponseDTO>(nutricionista);
    }

    public async Task<NutricionistaResponseDTO> ModificarNutricionista(int IdNutricionista, NutricionistaRequestDTO dto)
    {
        var nutriExiste = await _context.Nutricionistas
            .FirstOrDefaultAsync(n => n.Id_Nutricionista == IdNutricionista)
            ?? throw new ResourceNotFoundException("Nutricionista no encontrado.");

        _nutricionistaMapper.Map(dto, nutriExiste);

        _context.Nutricionistas.Update(nutriExiste);
        await _context.SaveChangesAsync();

        return await ObtenerNutricionistaConDetalles(IdNutricionista);
    }

    private async Task<NutricionistaResponseDTO> ObtenerNutricionistaConDetalles(int IdNutricionista)
    {
        var nutricionista = await _context.Nutricionistas
            .Include(n => n.Usuario)
            .FirstOrDefaultAsync(n => n.Id_Nutricionista == IdNutricionista)
            ?? throw new ResourceNotFoundException("Nutricionista no encontrado.");

        return _nutricionistaMapper.Map<NutricionistaResponseDTO>(nutricionista);
    }
}
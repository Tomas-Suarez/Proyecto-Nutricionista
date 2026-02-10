using AutoMapper;
using backend.Data;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Exceptions;
using backend.Models;
using backend.Service.Common;
using Microsoft.EntityFrameworkCore;

namespace backend.Service.imp;

public class PatologiaService : IPatologiaService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public PatologiaService(AppDbContext context, IMapper mapper, ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<PatologiaResponseDTO> Crear(PatologiaRequestDTO dto)
    {
        if (!_currentUserService.IsAdmin())
        {
            throw new AccessDeniedException("Solo el administrador puede crear patologías.");
        }

        bool existe = await _context.Patologias
            .AnyAsync(p => p.Nombre.ToLower() == dto.Nombre.ToLower());

        if (existe)
        {
            throw new DuplicateResourceException($"La patología '{dto.Nombre}' ya existe.");
        }

        var patologia = _mapper.Map<PatologiaEntity>(dto);
        _context.Patologias.Add(patologia);
        await _context.SaveChangesAsync();

        return _mapper.Map<PatologiaResponseDTO>(patologia);
    }

    public async Task<PatologiaResponseDTO> Modificar(int id, PatologiaRequestDTO dto)
    {
        if (!_currentUserService.IsAdmin())
        {
            throw new AccessDeniedException("Solo el administrador puede modificar patologías.");
        }

        var patologia = await _context.Patologias.FindAsync(id)
            ?? throw new ResourceNotFoundException("Patología no encontrada.");

        bool nombreOcupado = await _context.Patologias
            .AnyAsync(p => p.Nombre.ToLower() == dto.Nombre.ToLower() && p.Id_Patologia != id);

        if (nombreOcupado)
        {
            throw new DuplicateResourceException($"Ya existe otra patología con el nombre '{dto.Nombre}'.");
        }

        patologia.Nombre = dto.Nombre;

        await _context.SaveChangesAsync();

        return _mapper.Map<PatologiaResponseDTO>(patologia);
    }

    public async Task Eliminar(int id)
    {
        if (!_currentUserService.IsAdmin())
        {
            throw new AccessDeniedException("Solo el administrador puede eliminar patologías.");
        }

        var patologia = await _context.Patologias.FindAsync(id)
            ?? throw new ResourceNotFoundException($"No se encontró patología con el id: {id}");

        bool enUso = await _context.PatologiaPacientes.AnyAsync(pp => pp.Id_Patologia == id);
        if (enUso)
        {
            throw new BadRequestException("No se puede eliminar la patología porque hay pacientes asignados a ella.");
        }

        _context.Patologias.Remove(patologia);
        await _context.SaveChangesAsync();
    }

    public async Task<PatologiaResponseDTO> ObtenerPorId(int id)
    {
        var patologia = await _context.Patologias.FindAsync(id)
            ?? throw new ResourceNotFoundException($"No se encontró patología con el id: {id}");

        return _mapper.Map<PatologiaResponseDTO>(patologia);
    }

    public async Task<List<PatologiaResponseDTO>> ListarTodas()
    {
        var lista = await _context.Patologias
            .OrderBy(p => p.Nombre)
            .ToListAsync();

        return _mapper.Map<List<PatologiaResponseDTO>>(lista);
    }
}

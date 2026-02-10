using AutoMapper;
using backend.Data;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Exceptions;
using backend.Models;
using backend.Service.Common;
using Microsoft.EntityFrameworkCore;

namespace backend.Service.imp;

public class ObjetivoService : IObjetivoService
{

    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public ObjetivoService(AppDbContext context, IMapper mapper, ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }
    public async Task<ObjetivoResponseDTO> Crear(ObjetivoRequestDTO dto)
    {
        if (!_currentUserService.IsAdmin())
        {
            throw new AccessDeniedException("Acceso denegado.");
        }

        bool existe = await _context.Objetivos
            .AnyAsync(o => o.Nombre.ToLower() == dto.Nombre.ToLower());

        if (existe)
        {
            throw new DuplicateResourceException($"El objetivo '{dto.Nombre}' ya existe.");
        }

        var objetivo = _mapper.Map<ObjetivoEntity>(dto);
        _context.Objetivos.Add(objetivo);
        await _context.SaveChangesAsync();

        return _mapper.Map<ObjetivoResponseDTO>(objetivo);
    }

    public async Task Eliminar(int id)
    {
        if (!_currentUserService.IsAdmin())
        {
            throw new AccessDeniedException("Acceso denegado.");
        }

        var objetivo = await _context.Objetivos.FindAsync(id)
            ?? throw new ResourceNotFoundException($"No se encontró objetivo con el id: {id}");

        bool enUso = await _context.Pacientes.AnyAsync(p => p.Id_Objetivo == id);
        if (enUso)
        {
            throw new BadRequestException("No se puede eliminar el objetivo porque hay pacientes usandolo.");
        }

        _context.Objetivos.Remove(objetivo);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ObjetivoResponseDTO>> ListarTodos()
    {
        var lista = await _context.Objetivos
            .OrderBy(o => o.Nombre)
            .ToListAsync();
        return _mapper.Map<List<ObjetivoResponseDTO>>(lista);
    }

    public async Task<ObjetivoResponseDTO> Modificar(int id, ObjetivoRequestDTO dto)
    {
        if (!_currentUserService.IsAdmin()) throw new AccessDeniedException("Acceso denegado.");

        var objetivo = await _context.Objetivos.FindAsync(id)
            ?? throw new ResourceNotFoundException($"No se encontró objetivo con el id: {id}");

        bool duplicado = await _context.Objetivos
            .AnyAsync(o => o.Nombre.ToLower() == dto.Nombre.ToLower() && o.Id_Objetivo != id);

        if (duplicado) throw new DuplicateResourceException($"Ya existe un objetivo con ese nombre.");

        objetivo.Nombre = dto.Nombre;
        await _context.SaveChangesAsync();

        return _mapper.Map<ObjetivoResponseDTO>(objetivo);
    }

    public async Task<ObjetivoResponseDTO> ObtenerPorId(int id)
    {
        var entidad = await _context.Objetivos.FindAsync(id)
            ?? throw new ResourceNotFoundException($"No se encontró objetivo con el id: {id}");
        return _mapper.Map<ObjetivoResponseDTO>(entidad);
    }
}

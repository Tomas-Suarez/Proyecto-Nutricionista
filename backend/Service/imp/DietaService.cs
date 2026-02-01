using AutoMapper;
using backend.Data;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Exceptions;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Service.imp;

public class DietaService : IDietaService
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public DietaService(IMapper mapper, AppDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<DietaResponseDTO> CrearDieta(DietaRequestDTO dto)
    {
        var paciente = await _context.Pacientes
            .FirstOrDefaultAsync(p => p.Id_Paciente == dto.Id_Paciente)
            ?? throw new ResourceNotFoundException($"No se encontró paciente con el id: {dto.Id_Paciente}");

        _ = await _context.Nutricionistas.FindAsync(dto.Id_Nutricionista)
            ?? throw new ResourceNotFoundException($"Nutricionista con ID {dto.Id_Nutricionista} no encontrado.");

        var dietasActivasAnteriores = await _context.Dietas
            .Where(d => d.Id_Paciente == dto.Id_Paciente && d.Activa)
            .ToListAsync();

        foreach (var dietaVieja in dietasActivasAnteriores)
        {
            dietaVieja.Activa = false;
        }

        var nuevaDieta = _mapper.Map<DietaEntity>(dto);
        nuevaDieta.Activa = true;

        foreach (var comidaDto in dto.Comidas)
        {
            var comidaEnDb = await _context.Comidas.FindAsync(comidaDto.Id_Comida)
                ?? throw new ResourceNotFoundException($"Comida ID {comidaDto.Id_Comida} no encontrada.");

            nuevaDieta.DietaComidas.Add(new DietaComidaEntity
            {
                Comida = comidaEnDb,
                Cantidad = comidaDto.Cantidad,
                Horario = comidaDto.Horario,
                Nota = comidaDto.Nota
            });
        }

        _context.Dietas.Add(nuevaDieta);
        await _context.SaveChangesAsync();

        return _mapper.Map<DietaResponseDTO>(nuevaDieta);
    }

    public async Task<DietaResponseDTO> ObtenerPorId(int idDieta)
    {
        var dieta = await _context.Dietas
                .Include(d => d.Paciente)
                .Include(d => d.DietaComidas)
                    .ThenInclude(dc => dc.Comida)
                .FirstOrDefaultAsync(d => d.Id_Dieta == idDieta)
                ?? throw new ResourceNotFoundException($"No se encontró la dieta con id: {idDieta}");

        return _mapper.Map<DietaResponseDTO>(dieta);
    }

    public async Task<DietaResponseDTO> ActualizarDieta(int idDieta, DietaRequestDTO dto)
    {
        var dietaExistente = await _context.Dietas
            .Include(d => d.DietaComidas)
            .FirstOrDefaultAsync(d => d.Id_Dieta == idDieta)
            ?? throw new ResourceNotFoundException($"No se encontró la dieta con id: {idDieta}");

        _mapper.Map(dto, dietaExistente);

        dietaExistente.DietaComidas.Clear();

        foreach (var comidaDto in dto.Comidas)
        {
            var comidaEnDb = await _context.Comidas.FindAsync(comidaDto.Id_Comida)
                ?? throw new ResourceNotFoundException($"Comida {comidaDto.Id_Comida} no encontrada.");

            dietaExistente.DietaComidas.Add(new DietaComidaEntity
            {
                Comida = comidaEnDb,
                Cantidad = comidaDto.Cantidad,
                Horario = comidaDto.Horario,
                Nota = comidaDto.Nota
            });
        }

        await _context.SaveChangesAsync();
        return await ObtenerPorId(idDieta);
    }

    public async Task ActivarDieta(int idDieta, int idPaciente)
    {
        var dietasPrevias = await _context.Dietas
            .Where(d => d.Id_Paciente == idPaciente && d.Activa)
            .ToListAsync();

        foreach (var d in dietasPrevias) d.Activa = false;

        var dietaAActivar = await _context.Dietas.FindAsync(idDieta)
            ?? throw new ResourceNotFoundException($"No se encontró la dieta con id: {idDieta}");

        dietaAActivar.Activa = true;

        await _context.SaveChangesAsync();
    }

    public async Task EliminarDieta(int idDieta)
    {
        var dieta = await _context.Dietas.FindAsync(idDieta)
            ?? throw new ResourceNotFoundException($"No se encontró la dieta con id: {idDieta}");

        _context.Dietas.Remove(dieta);
        await _context.SaveChangesAsync();
    }

    public async Task<DietaResponseDTO?> ObtenerDietaActiva(int idPaciente)
    {
        var dietaActiva = await _context.Dietas
            .Include(d => d.Paciente)
            .Include(d => d.DietaComidas)
                .ThenInclude(dc => dc.Comida)
            .FirstOrDefaultAsync(d => d.Id_Paciente == idPaciente && d.Activa);

        return dietaActiva != null ? _mapper.Map<DietaResponseDTO>(dietaActiva) : null;
    }

    public async Task<IEnumerable<DietaResponseDTO>> ObtenerHistorialPaciente(int idPaciente)
    {
        var historial = await _context.Dietas
            .Include(d => d.Paciente)
            .Include(d => d.DietaComidas)
                .ThenInclude(dc => dc.Comida)
            .Where(d => d.Id_Paciente == idPaciente)
            .OrderByDescending(d => d.Fecha_Inicio)
            .ToListAsync();

        return _mapper.Map<IEnumerable<DietaResponseDTO>>(historial);
    }
}

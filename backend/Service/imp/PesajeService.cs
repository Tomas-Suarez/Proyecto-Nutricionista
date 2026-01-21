using AutoMapper;
using backend.Data;
using backend.Dtos.Common;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Exceptions;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Service.imp;

public class PesajeService : IPesajeService
{

    private readonly IMapper _mapper;

    private readonly AppDbContext _context;

    public PesajeService(IMapper mapper, AppDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<PesajeResponseDTO> RegistrarPesaje(PesajeRequestDTO dto)
    {
        var paciente = await _context.Pacientes
            .FirstOrDefaultAsync(p => p.Id_Paciente == dto.Id_Paciente)
            ?? throw new ResourceNotFoundException($"No se encontró paciente con el id: {dto.Id_Paciente}");

        var ultimoPesaje = await _context.Pesajes
                .Where(p => p.Id_Paciente == dto.Id_Paciente)
                .OrderByDescending(p => p.Fecha_Pesaje)
                .FirstOrDefaultAsync();

        var nuevoPesaje = _mapper.Map<PesajeEntity>(dto);
        _context.Pesajes.Add(nuevoPesaje);
        await _context.SaveChangesAsync();

        nuevoPesaje.Paciente = paciente;

        decimal pesoReferencia = ultimoPesaje?.Peso_Kg ?? paciente.Peso_Inicial;

        var response = _mapper.Map<PesajeResponseDTO>(nuevoPesaje);

        return response with { DiferenciaPesoAnterior = nuevoPesaje.Peso_Kg - pesoReferencia };

    }
    public async Task EliminarPesaje(int idPesaje)
    {
        var pesaje = await _context.Pesajes
            .FirstOrDefaultAsync(p => p.Id_Pesaje == idPesaje)
            ?? throw new ResourceNotFoundException($"No se encontró pesaje con el id: {idPesaje}");

        _context.Remove(pesaje);
        await _context.SaveChangesAsync();
    }

    public async Task<PagedResponseDTO<PesajeResponseDTO>> ObtenerHistorialPesaje(int idPaciente, int page, int size)
    {
        var query = _context.Pesajes
            .Where(p => p.Id_Paciente == idPaciente)
            .Include(p => p.Paciente)
            .OrderByDescending(p => p.Fecha_Pesaje);

        var totalRegistros = await query.CountAsync();

        var pesajes = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var pesajesDto = _mapper.Map<IEnumerable<PesajeResponseDTO>>(pesajes);

        return new PagedResponseDTO<PesajeResponseDTO>(pesajesDto, totalRegistros, page, size);
    }

    public async Task<PesajeResponseDTO> ObtenerPesajePorId(int idPesaje)
    {
        var pesaje = await _context.Pesajes
                .Include(p => p.Paciente)
                .FirstOrDefaultAsync(p => p.Id_Pesaje == idPesaje)
                ?? throw new ResourceNotFoundException($"No se encontró pesaje con el id: {idPesaje}");

        return _mapper.Map<PesajeResponseDTO>(pesaje);
    }
}
using AutoMapper;
using backend.Data;
using backend.Dtos.Common;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Exceptions;
using backend.Models;
using backend.Service.Common;
using Microsoft.EntityFrameworkCore;

namespace backend.Service.imp;

public class PesajeService : IPesajeService
{

    private readonly IMapper _mapper;
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public PesajeService(IMapper mapper, AppDbContext context, ICurrentUserService currentUserService)
    {
        _mapper = mapper;
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<PesajeResponseDTO> RegistrarPesaje(PesajeRequestDTO dto)
    {
        var paciente = await ObtenerPacienteAutorizado(dto.Id_Paciente);

        var ultimoPesaje = await _context.Pesajes
                .Where(p => p.Id_Paciente == paciente.Id_Paciente)
                .OrderByDescending(p => p.Fecha_Pesaje)
                .FirstOrDefaultAsync();

        var nuevoPesaje = _mapper.Map<PesajeEntity>(dto);

        nuevoPesaje.Id_Paciente = paciente.Id_Paciente;

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
            .Include(p => p.Paciente)
            .FirstOrDefaultAsync(p => p.Id_Pesaje == idPesaje)
            ?? throw new ResourceNotFoundException($"No se encontró pesaje con el id: {idPesaje}");

        await ValidarAccesoPaciente(pesaje.Paciente);

        _context.Remove(pesaje);
        await _context.SaveChangesAsync();
    }

    public async Task<PagedResponseDTO<PesajeResponseDTO>> ObtenerHistorialPorPaciente(int idPaciente, int page, int size, int? dias = null)
    {
        if (!_currentUserService.IsNutricionista() && !_currentUserService.IsAdmin())
        {
            throw new AccessDeniedException("Acceso denegado. Los pacientes deben usar el enlace público.");
        }

        await ObtenerPacienteAutorizado(idPaciente);

        return await ConsultarPesajesPaginados(idPaciente, page, size, dias);
    }

    public async Task<PagedResponseDTO<PesajeResponseDTO>> ObtenerHistorialPublico(string token, string codigo, int page, int size, int? dias = null)
    {
        var paciente = await _context.Pacientes
            .FirstOrDefaultAsync(p => p.TokenAcceso == token);

        if (paciente == null)
        {
            throw new ResourceNotFoundException("El enlace es inválido o ha expirado.");
        }

        if (paciente.CodigoAcceso != codigo)
        {
            throw new AccessDeniedException("El código de acceso es incorrecto.");
        }

        return await ConsultarPesajesPaginados(paciente.Id_Paciente, page, size, dias);
    }

    public async Task<PesajeResponseDTO> ObtenerPesajePorId(int idPesaje)
    {
        var pesaje = await _context.Pesajes
            .Include(p => p.Paciente)
            .FirstOrDefaultAsync(p => p.Id_Pesaje == idPesaje)
            ?? throw new ResourceNotFoundException($"No se encontró pesaje con el id: {idPesaje}");

        await ValidarAccesoPaciente(pesaje.Paciente);


        return _mapper.Map<PesajeResponseDTO>(pesaje);
    }

    private async Task<PacienteEntity> ObtenerPacienteAutorizado(int idPaciente)
    {
        var userId = _currentUserService.GetUserId()
            ?? throw new UnauthenticatedException("Usuario no autenticado...");

        if (_currentUserService.IsAdmin())
        {
            return await _context.Pacientes
                .FirstOrDefaultAsync(p => p.Id_Paciente == idPaciente)
                ?? throw new ResourceNotFoundException($"No se encontró paciente con el id: {idPaciente}");
        }

        if (_currentUserService.IsNutricionista())
        {
            var nutricionistaId = await _context.Nutricionistas
                .Where(n => n.Id_Usuario == userId)
                .Select(n => n.Id_Nutricionista)
                .FirstOrDefaultAsync();

            return await _context.Pacientes
                .FirstOrDefaultAsync(p =>
                    p.Id_Paciente == idPaciente &&
                    p.Id_Nutricionista == nutricionistaId)
                ?? throw new AccessDeniedException("Paciente no autorizado.");
        }

        throw new AccessDeniedException("Operación no permitida para usuarios estándar.");
    }

    private async Task ValidarAccesoPaciente(PacienteEntity paciente)
    {
        var userId = _currentUserService.GetUserId()
            ?? throw new UnauthenticatedException("Usuario no autenticado...");

        if (_currentUserService.IsAdmin())
            return;

        if (_currentUserService.IsNutricionista())
        {
            var nutricionistaId = await _context.Nutricionistas
                .Where(n => n.Id_Usuario == userId)
                .Select(n => n.Id_Nutricionista)
                .FirstOrDefaultAsync();

            if (paciente.Id_Nutricionista != nutricionistaId)
                throw new AccessDeniedException("No tienes acceso a este paciente.");

            return;
        }

        throw new AccessDeniedException("No tienes permiso para realizar esta acción.");
    }

    private async Task<PagedResponseDTO<PesajeResponseDTO>> ConsultarPesajesPaginados(int idPaciente, int page, int size, int? dias = null)
    {
        var query = _context.Pesajes
            .Where(p => p.Id_Paciente == idPaciente)
            .AsQueryable();

        if (dias.HasValue)
        {
            var fechaLimite = DateTime.UtcNow.AddDays(-dias.Value);
            query = query.Where(p => p.Fecha_Pesaje >= fechaLimite);
        }

        query = query.OrderByDescending(p => p.Fecha_Pesaje);

        var totalRegistros = await query.CountAsync();

        var pesajes = await query
            .Include(p => p.Paciente)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var pesajesDto = _mapper.Map<List<PesajeResponseDTO>>(pesajes);

        for (int i = 0; i < pesajesDto.Count; i++)
        {
            if (i + 1 < pesajesDto.Count)
            {
                decimal diferencia = pesajesDto[i].Peso_Kg - pesajesDto[i + 1].Peso_Kg;
                pesajesDto[i] = pesajesDto[i] with { DiferenciaPesoAnterior = diferencia };
            }
        }

        return new PagedResponseDTO<PesajeResponseDTO>(pesajesDto, totalRegistros, page, size);
    }
}
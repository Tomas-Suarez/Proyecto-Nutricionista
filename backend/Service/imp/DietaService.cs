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

public class DietaService : IDietaService
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DietaService(IMapper mapper, AppDbContext context, ICurrentUserService currentUserService)
    {
        _mapper = mapper;
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<DietaResponseDTO> CrearDieta(DietaRequestDTO dto)
    {
        var paciente = await ObtenerPacienteAutorizado(dto.Id_Paciente);

        var userId = _currentUserService.GetUserId()
        ?? throw new UnauthenticatedException("Usuario no autenticado.");

        var nutricionistaId = await _context.Nutricionistas
            .Where(n => n.Id_Usuario == userId)
            .Select(n => n.Id_Nutricionista)
            .FirstOrDefaultAsync();

        if (nutricionistaId == 0)
        {
            throw new AccessDeniedException("No se encontró el perfil de nutricionista asociado.");
        }

        if (dto.Activa)
        {
            var dietasActivasAnteriores = await _context.Dietas
                .Where(d => d.Id_Paciente == dto.Id_Paciente && d.Activa)
                .ToListAsync();

            foreach (var dietaVieja in dietasActivasAnteriores)
            {
                dietaVieja.Activa = false;
            }
        }

        var nuevaDieta = _mapper.Map<DietaEntity>(dto);
        nuevaDieta.Id_Paciente = paciente.Id_Paciente;

        nuevaDieta.Id_Paciente = paciente.Id_Paciente;
        nuevaDieta.Id_Nutricionista = nutricionistaId;

        foreach (var comidaDto in dto.Comidas)
        {
            var comidaEnDb = await _context.Comidas.FindAsync(comidaDto.Id_Comida)
                ?? throw new ResourceNotFoundException($"No se encontró la comida con id: {comidaDto.Id_Comida}");

            nuevaDieta.DietaComidas.Add(new DietaComidaEntity
            {
                Comida = comidaEnDb,
                Cantidad = comidaDto.Cantidad,
                Es_Permitido = comidaDto.Es_Permitido,
                Dia = comidaDto.Dia,
                Momento = comidaDto.Momento
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

        await ValidarAccesoPaciente(dieta.Paciente);

        return _mapper.Map<DietaResponseDTO>(dieta);
    }

    public async Task<DietaResponseDTO> ActualizarDieta(int idDieta, DietaRequestDTO dto)
    {
        var dietaExistente = await _context.Dietas
            .Include(d => d.Paciente)
            .Include(d => d.DietaComidas)
            .FirstOrDefaultAsync(d => d.Id_Dieta == idDieta)
            ?? throw new ResourceNotFoundException($"No se encontró la dieta con id: {idDieta}");

        await ValidarAccesoPaciente(dietaExistente.Paciente);

        var idPacienteOriginal = dietaExistente.Id_Paciente;
        var idNutricionistaOriginal = dietaExistente.Id_Nutricionista;

        _mapper.Map(dto, dietaExistente);

        dietaExistente.Id_Paciente = idPacienteOriginal;
        dietaExistente.Id_Nutricionista = idNutricionistaOriginal;

        if (dietaExistente.Activa)
        {
            var otrasActivas = await _context.Dietas
                .Where(d => d.Id_Paciente == dietaExistente.Id_Paciente && d.Id_Dieta != idDieta && d.Activa)
                .ToListAsync();

            foreach (var d in otrasActivas) d.Activa = false;
        }

        dietaExistente.DietaComidas.Clear();

        foreach (var comidaDto in dto.Comidas)
        {
            var comidaEnDb = await _context.Comidas.FindAsync(comidaDto.Id_Comida)
                ?? throw new ResourceNotFoundException($"Comida {comidaDto.Id_Comida} no encontrada.");

            dietaExistente.DietaComidas.Add(new DietaComidaEntity
            {
                Comida = comidaEnDb,
                Cantidad = comidaDto.Cantidad,
                Es_Permitido = comidaDto.Es_Permitido,
                Dia = comidaDto.Dia,
                Momento = comidaDto.Momento
            });
        }

        await _context.SaveChangesAsync();

        return await ObtenerPorId(idDieta);
    }

    public async Task ActivarDieta(int idDieta)
    {
        var dieta = await _context.Dietas
            .Include(d => d.Paciente)
            .FirstOrDefaultAsync(d => d.Id_Dieta == idDieta)
            ?? throw new ResourceNotFoundException($"No se encontró la dieta con id: {idDieta}");

        await ValidarAccesoPaciente(dieta.Paciente);

        var dietasPrevias = await _context.Dietas
            .Where(d => d.Id_Paciente == dieta.Id_Paciente && d.Activa)
            .ToListAsync();

        foreach (var d in dietasPrevias)
            d.Activa = false;

        dieta.Activa = true;

        await _context.SaveChangesAsync();
    }


    public async Task EliminarDieta(int idDieta)
    {
        var dieta = await _context.Dietas
            .Include(d => d.Paciente)
            .FirstOrDefaultAsync(d => d.Id_Dieta == idDieta)
            ?? throw new ResourceNotFoundException($"No se encontró la dieta con id: {idDieta}");

        await ValidarAccesoPaciente(dieta.Paciente);

        _context.Dietas.Remove(dieta);
        await _context.SaveChangesAsync();
    }

    public async Task<DietaResponseDTO?> ObtenerDietaActiva(int idPaciente)
    {
        var paciente = await ObtenerPacienteAutorizado(idPaciente);

        var dietaActiva = await _context.Dietas
            .Include(d => d.Paciente)
            .Include(d => d.DietaComidas)
                .ThenInclude(dc => dc.Comida)
            .FirstOrDefaultAsync(d => d.Id_Paciente == idPaciente && d.Activa);

        return dietaActiva != null ? _mapper.Map<DietaResponseDTO>(dietaActiva) : null;
    }

    public async Task<PagedResponseDTO<DietaResponseDTO>> ObtenerHistorialPaciente(int idPaciente, int page, int size)
    {
        await ObtenerPacienteAutorizado(idPaciente);

        var query = _context.Dietas
            .Include(d => d.Paciente)
            .Include(d => d.DietaComidas)
                .ThenInclude(dc => dc.Comida)
            .Where(d => d.Id_Paciente == idPaciente);

        var totalCount = await query.CountAsync();

        var items = await query
                .OrderByDescending(d => d.Activa)
                .ThenByDescending(d => d.Fecha_Inicio)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

        var dtos = _mapper.Map<IEnumerable<DietaResponseDTO>>(items);

        return new PagedResponseDTO<DietaResponseDTO>(
            dtos,
            totalCount,
            page,
            size
        );
    }

    public async Task<DietaResponseDTO?> ObtenerDietaActualPublica(string token, string codigo)
    {
        var paciente = await _context.Pacientes
            .FirstOrDefaultAsync(p => p.TokenAcceso == token);

        if (paciente == null)
        {
            throw new ResourceNotFoundException("Enlace inválido.");
        }

        if (paciente.CodigoAcceso != codigo)
        {
            throw new AccessDeniedException("Código incorrecto.");
        }

        var dietaActiva = await _context.Dietas
            .Include(d => d.DietaComidas)
                .ThenInclude(dc => dc.Comida)
            .FirstOrDefaultAsync(d => d.Id_Paciente == paciente.Id_Paciente && d.Activa);

        return dietaActiva != null ? _mapper.Map<DietaResponseDTO>(dietaActiva) : null;
    }

    private async Task<PacienteEntity> ObtenerPacienteAutorizado(int idPaciente)
    {
        var userId = _currentUserService.GetUserId()
            ?? throw new UnauthenticatedException("Usuario no autenticado.");

        if (_currentUserService.IsAdmin())
        {
            return await _context.Pacientes.FindAsync(idPaciente)
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

        throw new AccessDeniedException("Acceso restringido a profesionales.");
    }

    private async Task ValidarAccesoPaciente(PacienteEntity paciente)
    {
        await ObtenerPacienteAutorizado(paciente.Id_Paciente);
    }
}

using AutoMapper;
using backend.Data;
using backend.Dtos.Common;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Enum;
using backend.Exceptions;
using backend.Models;
using backend.Service.Common;
using Microsoft.EntityFrameworkCore;

namespace backend.Service.imp;

public class PacienteService : IPacienteService
{

    private readonly AppDbContext _context;
    private readonly IMapper _pacienteMapper;
    private readonly ICurrentUserService _currentUserService;

    public PacienteService(AppDbContext context, IMapper pacienteMapper, ICurrentUserService currentUserService)
    {
        _context = context;
        _pacienteMapper = pacienteMapper;
        _currentUserService = currentUserService;
    }

    public async Task<PacienteResponseDTO> RegistrarPaciente(RegistroPacienteDTO dto)
    {
        if (!_currentUserService.IsNutricionista() && !_currentUserService.IsAdmin())
        {
            throw new AccessDeniedException("Solo los nutricionistas pueden registrar pacientes.");
        }

        var userId = _currentUserService.GetUserId()
            ?? throw new UnauthenticatedException("Usuario no autenticado...");

        var nutricionista = await _context.Nutricionistas
            .FirstOrDefaultAsync(n => n.Id_Usuario == userId)
            ?? throw new ResourceNotFoundException("No se encontró tu perfil de profesional.");

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            bool yaEsPaciente = await _context.Pacientes
                .AnyAsync(p => p.Email == dto.Email && p.Id_Nutricionista == nutricionista.Id_Nutricionista);

            if (yaEsPaciente)
            {
                throw new DuplicateResourceException("Ya tienes un paciente registrado con ese email.");
            }

            var paciente = _pacienteMapper.Map<PacienteEntity>(dto);

            paciente.Id_Nutricionista = nutricionista.Id_Nutricionista;
            paciente.Estado = EEstadoPaciente.Activo;
            paciente.Email = dto.Email;

            paciente.TokenAcceso = Guid.NewGuid().ToString();
            paciente.CodigoAcceso = new Random().Next(1000, 9999).ToString();

            if (dto.IdsPatologias != null && dto.IdsPatologias.Count > 0)
            {
                paciente.PatologiaPacientes = new List<PatologiaPacienteEntity>();
                foreach (var idPatologia in dto.IdsPatologias)
                {
                    paciente.PatologiaPacientes.Add(new PatologiaPacienteEntity
                    {
                        Id_Patologia = idPatologia
                    });
                }
            }

            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return await ObtenerPacienteConDetalles(paciente.Id_Paciente);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PacienteResponseDTO> ModificarPaciente(int idPaciente, PacienteRequestDTO dto)
    {
        var paciente = await _context.Pacientes
            .Include(p => p.PatologiaPacientes)
            .FirstOrDefaultAsync(p => p.Id_Paciente == idPaciente)
            ?? throw new ResourceNotFoundException($"No se encontró paciente con el id: {idPaciente}");

        ValidarAccesoPaciente(paciente);

        _pacienteMapper.Map(dto, paciente);

        var patologiasAEliminar = paciente.PatologiaPacientes
            .Where(pp => !dto.IdsPatologias.Contains(pp.Id_Patologia))
            .ToList();

        foreach (var pp in patologiasAEliminar)
        {
            paciente.PatologiaPacientes.Remove(pp);
        }

        var existentesIds = paciente.PatologiaPacientes.Select(pp => pp.Id_Patologia).ToList();
        foreach (var idPat in dto.IdsPatologias)
        {
            if (!existentesIds.Contains(idPat))
            {
                paciente.PatologiaPacientes.Add(new PatologiaPacienteEntity
                {
                    Id_Paciente = idPaciente,
                    Id_Patologia = idPat
                });
            }
        }

        await _context.SaveChangesAsync();

        return await ObtenerPacienteConDetalles(idPaciente);
    }

    public async Task CambiarEstado(int idPaciente, EEstadoPaciente nuevoEstado)
    {
        if (!System.Enum.IsDefined(typeof(EEstadoPaciente), nuevoEstado))
        {
            throw new BadRequestException($"El valor {nuevoEstado} no es un estado válido.");
        }

        var paciente = await _context.Pacientes
            .FirstOrDefaultAsync(p => p.Id_Paciente == idPaciente)
            ?? throw new ResourceNotFoundException($"No se encontró paciente con el id: {idPaciente}");

        ValidarAccesoPaciente(paciente);

        paciente.Estado = nuevoEstado;

        _context.Pacientes.Update(paciente);
        await _context.SaveChangesAsync();
    }

    public async Task<PagedResponseDTO<PacienteResponseDTO>> ObtenerPacientesPorNutricionista(
                int page, int size, EEstadoPaciente? estado, string? busqueda = null)
    {
        var userId = _currentUserService.GetUserId()
            ?? throw new UnauthenticatedException("Usuario no autenticado...");

        var nutricionistaId = await _context.Nutricionistas
            .Where(n => n.Id_Usuario == userId)
            .Select(n => n.Id_Nutricionista)
            .FirstOrDefaultAsync();

        var query = _context.Pacientes
            .Include(p => p.Objetivo)
            .Include(p => p.PatologiaPacientes)
                .ThenInclude(pp => pp.Patologia)
                .Include(p => p.Pesajes)
            .Where(p => p.Id_Nutricionista == nutricionistaId);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            query = query.Where(p =>
                p.Nombre.Contains(busqueda) ||
                p.Apellido.Contains(busqueda) ||
                p.Dni.Contains(busqueda)
            );
        }

        if (estado.HasValue)
        {
            query = query.Where(p => p.Estado == estado.Value);
        }

        var totalRegistros = await query.CountAsync();

        var pacientes = await query
            .OrderBy(p => p.Apellido)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var itemsDTO = _pacienteMapper.Map<IEnumerable<PacienteResponseDTO>>(pacientes);

        return new PagedResponseDTO<PacienteResponseDTO>(itemsDTO, totalRegistros, page, size);
    }

    private async Task<PacienteResponseDTO> ObtenerPacienteConDetalles(int idPaciente)
    {
        var paciente = await _context.Pacientes
            .Include(p => p.Objetivo)
            .Include(p => p.PatologiaPacientes).ThenInclude(pp => pp.Patologia)
            .FirstOrDefaultAsync(p => p.Id_Paciente == idPaciente)
            ?? throw new ResourceNotFoundException($"No se encontró paciente con el id: {idPaciente}");

        return _pacienteMapper.Map<PacienteResponseDTO>(paciente);
    }

    public async Task<PacienteResponseDTO> ValidarCredencialesPaciente(string token, string codigo)
    {
        var paciente = await _context.Pacientes
            .Include(p => p.Objetivo)
            .Include(p => p.PatologiaPacientes).ThenInclude(pp => pp.Patologia)
            .Include(p => p.Pesajes)
            .FirstOrDefaultAsync(p => p.TokenAcceso == token);

        if (paciente == null)
        {
            throw new ResourceNotFoundException("El enlace no es válido o ha expirado.");
        }

        if (paciente.CodigoAcceso != codigo)
        {
            throw new AccessDeniedException("El código de acceso es incorrecto.");
        }

        var baseResponse = _pacienteMapper.Map<PacienteResponseDTO>(paciente);

        var archivos = await _context.ArchivosNutricionistas
            .Where(a => a.Id_Nutricionista == paciente.Id_Nutricionista)
            .Select(a => new ArchivoResponseDTO(
                a.Id_Archivo,
                a.NombreArchivo,
                a.RutaAcceso,
                a.FechaSubida
            ))
            .ToListAsync();

        var historialEntities = await _context.Pesajes
            .Where(p => p.Id_Paciente == paciente.Id_Paciente)
            .OrderBy(p => p.Fecha_Pesaje)
            .ToListAsync();

        var historialDto = historialEntities.Select(p => new PesajeResponseDTO(
            p.Id_Pesaje,
            p.Id_Paciente,
            p.Peso_Kg,
            p.Porcentaje_Grasa,
            p.Masa_Muscular_Kg,
            p.Fecha_Pesaje,
            p.Nota,
            0,
            EIMC.Peso_Normal,
            0
        )).ToList();

        var dietaEntity = await _context.Dietas
            .Include(d => d.DietaComidas)
                .ThenInclude(dc => dc.Comida)
            .FirstOrDefaultAsync(d => d.Id_Paciente == paciente.Id_Paciente && d.Activa);

        DietaResponseDTO? dietaDto = null;
        if (dietaEntity != null)
        {
            dietaDto = _pacienteMapper.Map<DietaResponseDTO>(dietaEntity);
        }

        return baseResponse with
        {
            ArchivosNutricionista = archivos,
            HistorialPeso = historialDto,
            DietaActual = dietaDto
        };
    }

    public async Task<PacienteResponseDTO> ObtenerPorId(int idPaciente)
    {
        var paciente = await _context.Pacientes
            .Include(p => p.Objetivo)
            .Include(p => p.PatologiaPacientes)
                .ThenInclude(pp => pp.Patologia)
                .Include(p => p.Pesajes)
            .FirstOrDefaultAsync(p => p.Id_Paciente == idPaciente)
            ?? throw new ResourceNotFoundException($"No se encontró paciente con el id: {idPaciente}");

        ValidarAccesoPaciente(paciente);

        return _pacienteMapper.Map<PacienteResponseDTO>(paciente);
    }

    private void ValidarAccesoPaciente(PacienteEntity paciente)
    {
        var userId = _currentUserService.GetUserId();
        var isAdmin = _currentUserService.IsAdmin();

        if (isAdmin)
            return;

        var nutricionistaId = _context.Nutricionistas
            .Where(n => n.Id_Usuario == userId)
            .Select(n => n.Id_Nutricionista)
            .FirstOrDefault();

        if (paciente.Id_Nutricionista != nutricionistaId)
        {
            throw new AccessDeniedException("No tienes permiso para operar sobre este paciente.");
        }
    }


}
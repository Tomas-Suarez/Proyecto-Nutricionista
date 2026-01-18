using AutoMapper;
using backend.Data;
using backend.Dtos.Common;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Enum;
using backend.Exceptions;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Service.imp;

public class PacienteService : IPacienteService
{

    private readonly AppDbContext _context;
    private readonly IMapper _pacienteMapper;
    private readonly IUsuarioService _usuarioService;

    public PacienteService(AppDbContext context, IMapper pacienteMapper, IUsuarioService usuarioService)
    {
        _context = context;
        _pacienteMapper = pacienteMapper;
        _usuarioService = usuarioService;
    }

    public async Task<PacienteResponseDTO> RegistrarPaciente(RegistroPacienteDTO dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        var usuarioExiste = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        int idUsuario;

        if (usuarioExiste == null)
        {
            var usuarioDto = new UsuarioRequestDTO(
                dto.Email,
                dto.Dni,
                ERol.Paciente
                );

            var nuevoUsuario = await _usuarioService.RegistrarUsuario(usuarioDto);
            idUsuario = nuevoUsuario.Id_Usuario;
        }
        else
        {
            idUsuario = usuarioExiste.Id_Usuario;
            bool esPaciente = await _context.Pacientes
                .AnyAsync(p => p.Id_Usuario == idUsuario && p.Id_Nutricionista == dto.Id_Nutricionista);

            if (esPaciente)
            {
                throw new DuplicateResourceException("El paciente ya esta se encuentra registrado en su lista");
            }

            var paciente = _pacienteMapper.Map<PacienteEntity>(dto);
            paciente.Id_Usuario = idUsuario;
            paciente.Estado = EEstadoPaciente.Activo;

            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return await ObtenerPacienteConDetalles(paciente.Id_Paciente);
        }

        throw new NotImplementedException();
    }

    public async Task<PacienteResponseDTO> ObtenerPorUsuarioId(int idUsuario)
    {
        var paciente = await _context.Pacientes
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.Id_Usuario == idUsuario)
            ?? throw new ResourceNotFoundException("No se encontró el perfil de paciente para este usuario.");

        return _pacienteMapper.Map<PacienteResponseDTO>(paciente);
    }

    public async Task CambiarEstado(int idPaciente, EEstadoPaciente nuevoEstado)
    {
        var paciente = await _context.Pacientes
            .FirstOrDefaultAsync(p => p.Id_Paciente == idPaciente)
            ?? throw new ResourceNotFoundException($"No se encontró paciente con el id: {idPaciente}");

        paciente.Estado = nuevoEstado;

        _context.Pacientes.Update(paciente);
        await _context.SaveChangesAsync();
    }

    public async Task<PacienteResponseDTO> ModificarPaciente(int IdPaciente, PacienteRequestDTO dto)
    {
        var pacienteExiste = await _context.Pacientes
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.Id_Paciente == IdPaciente)
            ?? throw new ResourceNotFoundException($"No se encontró paciente con el id: {IdPaciente}");

        _pacienteMapper.Map(dto, pacienteExiste);
        _context.Pacientes.Update(pacienteExiste);
        await _context.SaveChangesAsync();

        return await ObtenerPacienteConDetalles(IdPaciente);
    }

    public async Task<PacienteResponseDTO> ObtenerPacientePorId(int idPaciente)
    {
        return await ObtenerPacienteConDetalles(idPaciente);
    }

    public async Task<PagedResponseDTO<PacienteResponseDTO>> ObtenerPacientesPorNutricionista(
        int idNutricionista, int page, int size)
    {
        var query = _context.Pacientes
            .Include(p => p.Usuario)
            .Where(p => p.Id_Nutricionista == idNutricionista && p.Estado == EEstadoPaciente.Activo);

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
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.Id_Paciente == idPaciente)
            ?? throw new ResourceNotFoundException($"No se encontró paciente con el id: {idPaciente}");

        return _pacienteMapper.Map<PacienteResponseDTO>(paciente);
    }
}
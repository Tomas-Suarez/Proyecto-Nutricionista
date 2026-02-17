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

public async Task<ArchivoResponseDTO> SubirPdf(SubirPdfRequestDTO dto)
    {
        var nutricionista = await ObtenerNutricionistaActual();
        int idNutricionista = nutricionista.Id_Nutricionista;

        if (dto.Archivo.ContentType != "application/pdf")
        {
            throw new BadRequestException("Solo se permiten archivos PDF.");
        }

        var cantidad = await _context.ArchivosNutricionistas
            .CountAsync(a => a.Id_Nutricionista == idNutricionista);

        if (cantidad >= 3)
        {
            throw new ConflictException("Has alcanzado el límite máximo de 3 archivos.");
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "pdfs", idNutricionista.ToString());
        
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{dto.Archivo.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.Archivo.CopyToAsync(stream);
        }

        var nuevoArchivo = new ArchivoNutricionistaEntity
        {
            Id_Nutricionista = idNutricionista,
            NombreArchivo = dto.NombrePersonalizado ?? dto.Archivo.FileName,
            RutaAcceso = $"/uploads/pdfs/{idNutricionista}/{uniqueFileName}",
            FechaSubida = DateTime.Now
        };

        _context.ArchivosNutricionistas.Add(nuevoArchivo);
        await _context.SaveChangesAsync();

        return new ArchivoResponseDTO(
            nuevoArchivo.Id_Archivo,
            nuevoArchivo.NombreArchivo,
            nuevoArchivo.RutaAcceso,
            nuevoArchivo.FechaSubida
        );
    }

    public async Task<List<ArchivoResponseDTO>> ObtenerMisArchivos()
    {
        var nutricionista = await ObtenerNutricionistaActual();

        return await _context.ArchivosNutricionistas
            .Where(a => a.Id_Nutricionista == nutricionista.Id_Nutricionista)
            .Select(a => new ArchivoResponseDTO(
                a.Id_Archivo,
                a.NombreArchivo,
                a.RutaAcceso,
                a.FechaSubida
            ))
            .ToListAsync();
    }

    public async Task EliminarArchivo(int idArchivo)
    {
        var nutricionista = await ObtenerNutricionistaActual();

        var archivo = await _context.ArchivosNutricionistas
            .FirstOrDefaultAsync(a => a.Id_Archivo == idArchivo && a.Id_Nutricionista == nutricionista.Id_Nutricionista)
            ?? throw new ResourceNotFoundException("Archivo no encontrado.");

        // Borrado Físico
        var rootPath = Directory.GetCurrentDirectory();
        var rutaRelativa = archivo.RutaAcceso.TrimStart('/', '\\'); 
        var rutaFisica = Path.Combine(rootPath, "wwwroot", rutaRelativa);

        if (File.Exists(rutaFisica))
        {
            File.Delete(rutaFisica);
        }

        _context.ArchivosNutricionistas.Remove(archivo);
        await _context.SaveChangesAsync();
    }

    private async Task<NutricionistaEntity> ObtenerNutricionistaActual()
    {
        var userId = _currentUserService.GetUserId()
             ?? throw new UnauthenticatedException("Usuario no autenticado.");

        var nutricionista = await _context.Nutricionistas
            .FirstOrDefaultAsync(n => n.Id_Usuario == userId);

        if (nutricionista == null)
        {
            throw new AccessDeniedException("No se encontró el perfil de nutricionista asociado.");
        }

        return nutricionista;
    }

}
using AutoMapper;
using backend.Data;
using backend.Dtos.Common;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Exceptions;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Service.imp;

public class ComidaService : IComidaService
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public ComidaService(IMapper mapper, AppDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<ComidaResponseDTO> RegistrarComida(ComidaRequestDTO dto)
    {
        var categorias = await _context.Categorias
            .Where(c => dto.Ids_Categorias.Contains(c.Id_Categoria))
            .ToListAsync();

        if (categorias.Count == 0)
        {
            throw new BadRequestException("Debe seleccionar categorías válidas.");
        }

        var nuevaComida = _mapper.Map<ComidaEntity>(dto);

        decimal calculoCalorias = (dto.Proteinas * 4) + (dto.Carbohidratos * 4) + (dto.Grasas * 9);
        nuevaComida.Calorias = (int)Math.Round(calculoCalorias);

        nuevaComida.Categorias = categorias;

        _context.Comidas.Add(nuevaComida);

        await _context.SaveChangesAsync();

        return _mapper.Map<ComidaResponseDTO>(nuevaComida);
    }

    public async Task<ComidaResponseDTO> ObtenerPorId(int idComida)
    {
        var comida = await _context.Comidas
            .Include(c => c.Categorias)
            .FirstOrDefaultAsync(c => c.Id_Comida == idComida)
            ?? throw new ResourceNotFoundException($"No se encontró la comida con el id: {idComida}");

        return _mapper.Map<ComidaResponseDTO>(comida);
    }

    public async Task<PagedResponseDTO<ComidaResponseDTO>> ObtenerTodas(int page, int size, int? idCategoria, string? nombre)
    {
        var query = _context.Comidas.Include(c => c.Categorias).AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            query = query.Where(c => c.Nombre.Contains(nombre));
        }

        if (idCategoria.HasValue)
        {
            query = query.Where(c => c.Categorias.Any(cat => cat.Id_Categoria == idCategoria.Value));
        }

        var totalRegistros = await query.CountAsync();

        var comidas = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var comidasDtos = _mapper.Map<IEnumerable<ComidaResponseDTO>>(comidas);

        return new PagedResponseDTO<ComidaResponseDTO>(comidasDtos, totalRegistros, page, size);
    }

    public async Task EliminarComida(int idComida)
    {
        var comida = await _context.Comidas.FindAsync(idComida)
            ?? throw new ResourceNotFoundException($"No se encontró la comida con el id: {idComida}");

        _context.Comidas.Remove(comida);
        await _context.SaveChangesAsync();
    }

    public async Task<ComidaResponseDTO> ActualizarComida(int idComida, ComidaRequestDTO dto)
    {
        var comidaExiste = await _context.Comidas
            .Include(c => c.Categorias)
            .FirstOrDefaultAsync(c => c.Id_Comida == idComida)
            ?? throw new ResourceNotFoundException($"No se encontró la comida con el id: {idComida}");

        var nuevaCategorias = await _context.Categorias
            .Where(c => dto.Ids_Categorias.Contains(c.Id_Categoria))
            .ToListAsync();

        if (nuevaCategorias.Count == 0)
        {
            throw new BadRequestException("Debe seleccionar categorías válidas.");
        }

        _mapper.Map(dto, comidaExiste);

        comidaExiste.Categorias.Clear();

        foreach (var cat in nuevaCategorias)
        {
            comidaExiste.Categorias.Add(cat);
        }

        decimal calculoCalorias = (dto.Proteinas * 4) + (dto.Carbohidratos * 4) + (dto.Grasas * 9);
        comidaExiste.Calorias = (int)Math.Round(calculoCalorias);

        await _context.SaveChangesAsync();

        return _mapper.Map<ComidaResponseDTO>(comidaExiste);
    }
}

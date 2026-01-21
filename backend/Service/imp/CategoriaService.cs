using AutoMapper;
using backend.Data;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Exceptions;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Service.imp;

public class CategoriaService : ICategoriaService
{
    private readonly AppDbContext _context;
    private readonly IMapper _categoriaMapper;

    public CategoriaService(AppDbContext context, IMapper categoriaMapper)
    {
        _context = context;
        _categoriaMapper = categoriaMapper;
    }

    public async Task<CategoriaResponseDTO> CrearCategoria(CategoriaRequestDTO dto)
    {
        var nuevaCategoria = _categoriaMapper.Map<CategoriaEntity>(dto);
        _context.Categorias.Add(nuevaCategoria);
        await _context.SaveChangesAsync();

        return _categoriaMapper.Map<CategoriaResponseDTO>(nuevaCategoria);
    }

    public async Task EliminarCategoria(int idCategoria)
    {
        var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Id_Categoria == idCategoria)
        ?? throw new ResourceNotFoundException($"No se encontró categoria con el id: {idCategoria}");

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CategoriaResponseDTO>> ObtenerTodasCategorias()
    {
        var categorias = await _context.Categorias.ToListAsync();

        return _categoriaMapper.Map<IEnumerable<CategoriaResponseDTO>>(categorias);
    }
}

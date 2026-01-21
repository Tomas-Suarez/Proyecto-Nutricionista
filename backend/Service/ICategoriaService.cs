using backend.Dtos.request;
using backend.Dtos.response;

namespace backend.Service;

public interface ICategoriaService
{
    Task<IEnumerable<CategoriaResponseDTO>> ObtenerTodasCategorias();
    Task<CategoriaResponseDTO> CrearCategoria(CategoriaRequestDTO dto);
    Task EliminarCategoria(int idCategoria);
    Task<CategoriaResponseDTO> ObtenerPorId(int idCategoria);
}
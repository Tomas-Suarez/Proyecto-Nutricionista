using backend.Dtos.Common;
using backend.Dtos.request;
using backend.Dtos.response;

namespace backend.Service;

public interface IObjetivoService
{
    Task<ObjetivoResponseDTO> Crear(ObjetivoRequestDTO dto);

    Task<ObjetivoResponseDTO> Modificar(int id, ObjetivoRequestDTO dto);

    Task Eliminar(int id);

    Task<List<ObjetivoResponseDTO>> ListarTodos();

    Task<ObjetivoResponseDTO> ObtenerPorId(int id);

    Task<PagedResponseDTO<ObjetivoResponseDTO>> ObtenerPaginado(int page, int size);
}

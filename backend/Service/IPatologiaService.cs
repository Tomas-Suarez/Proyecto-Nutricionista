using backend.Dtos.request;
using backend.Dtos.response;

namespace backend.Service;

public interface IPatologiaService
{
    Task<PatologiaResponseDTO> Crear(PatologiaRequestDTO dto);

    Task<PatologiaResponseDTO> Modificar(int id, PatologiaRequestDTO dto);

    Task Eliminar(int id);

    Task<List<PatologiaResponseDTO>> ListarTodas();
    
    Task<PatologiaResponseDTO> ObtenerPorId(int id);
}

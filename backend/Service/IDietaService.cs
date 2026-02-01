using backend.Dtos.request;
using backend.Dtos.response;

namespace backend.Service;

public interface IDietaService
{
    Task<DietaResponseDTO> CrearDieta(DietaRequestDTO dto);
    
    Task<DietaResponseDTO> ActualizarDieta(int idDieta, DietaRequestDTO dto);

    Task EliminarDieta(int idDieta);

    Task<DietaResponseDTO> ObtenerPorId(int idDieta);

    Task<IEnumerable<DietaResponseDTO>> ObtenerHistorialPaciente(int idPaciente);

    Task<DietaResponseDTO?> ObtenerDietaActiva(int idPaciente);

    Task ActivarDieta(int idDieta, int idPaciente); 
}

using backend.Dtos.Common;
using backend.Dtos.request;
using backend.Dtos.response;

namespace backend.Service;

public interface IDietaService
{
    Task<DietaResponseDTO> CrearDieta(DietaRequestDTO dto);

    Task<DietaResponseDTO> ActualizarDieta(int idDieta, DietaRequestDTO dto);

    Task EliminarDieta(int idDieta);

    Task<DietaResponseDTO> ObtenerPorId(int idDieta);

    Task<PagedResponseDTO<DietaResponseDTO>> ObtenerHistorialPaciente(int idPaciente, int page, int size);
    
    Task<DietaResponseDTO?> ObtenerDietaActiva(int idPaciente);

    Task ActivarDieta(int idDieta);

    Task<DietaResponseDTO?> ObtenerDietaActualPublica(string token, string codigo);
}

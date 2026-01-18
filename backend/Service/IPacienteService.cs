using backend.Dtos.Common;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Enum;

namespace backend.Service;

public interface IPacienteService
{
        Task<PacienteResponseDTO> RegistrarPaciente(RegistroPacienteDTO dto);
        
        Task<PacienteResponseDTO> ObtenerPacientePorId(int idPaciente);

        Task<PacienteResponseDTO> ObtenerPorUsuarioId(int idUsuario);

        Task<PacienteResponseDTO> ModificarPaciente(int IdPaciente, PacienteRequestDTO dto);

        Task<PagedResponseDTO<PacienteResponseDTO>> ObtenerPacientesPorNutricionista(int idNutricionista, int page, int size);

        Task CambiarEstado(int idPaciente, EEstadoPaciente nuevoEstado);
}

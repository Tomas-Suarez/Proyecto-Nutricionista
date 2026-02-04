using backend.Dtos.Common;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Enum;

namespace backend.Service;

public interface IPacienteService
{
        Task<PacienteResponseDTO> RegistrarPaciente(RegistroPacienteDTO dto);
        
        Task<PacienteResponseDTO> ObtenerMiPerfil();

        Task<PacienteResponseDTO> ModificarMiPerfil(PacienteRequestDTO dto);

        Task<PacienteResponseDTO> ModificarPaciente(int IdPaciente, PacienteRequestDTO dto);

        Task<PagedResponseDTO<PacienteResponseDTO>> ObtenerPacientesPorNutricionista(int page, int size, EEstadoPaciente? estado);

        Task CambiarEstado(int idPaciente, EEstadoPaciente nuevoEstado);
}

using backend.Dtos.Common;
using backend.Dtos.request;
using backend.Dtos.response;
using backend.Enum;

namespace backend.Service;

public interface IPacienteService
{
        Task<PacienteResponseDTO> RegistrarPaciente(RegistroPacienteDTO dto);

        Task<PacienteResponseDTO> ModificarPaciente(int idPaciente, PacienteRequestDTO dto);

        Task<PagedResponseDTO<PacienteResponseDTO>> ObtenerPacientesPorNutricionista(
            int page, int size, EEstadoPaciente? estado, string? busqueda = null);

        Task CambiarEstado(int idPaciente, EEstadoPaciente nuevoEstado);

        Task<PacienteResponseDTO> ValidarCredencialesPaciente(string token, string codigo);
}

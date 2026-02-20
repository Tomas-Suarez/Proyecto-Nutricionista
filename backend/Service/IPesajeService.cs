using backend.Dtos.Common;
using backend.Dtos.request;
using backend.Dtos.response;

namespace backend.Service;

public interface IPesajeService
{
    Task<PesajeResponseDTO> RegistrarPesaje(PesajeRequestDTO dto);

    Task<PagedResponseDTO<PesajeResponseDTO>> ObtenerHistorialPorPaciente(int idPaciente, int page, int size, int? dias = null);

    Task<PagedResponseDTO<PesajeResponseDTO>> ObtenerHistorialPublico(string token, string codigo, int page, int size, int? dias = null);
    
    Task<PesajeResponseDTO> ObtenerPesajePorId(int idPesaje);

    Task EliminarPesaje(int idPesaje);
}

using backend.Dtos.Common;
using backend.Dtos.request;
using backend.Dtos.response;

namespace backend.Service;

public interface IPesajeService
{
    Task<PesajeResponseDTO> RegistrarPesaje(PesajeRequestDTO dto);

    Task<PagedResponseDTO<PesajeResponseDTO>> ObtenerHistorialPesaje(int idPaciente, int page, int size);

    Task<PesajeResponseDTO> ObtenerPesajePorId(int idPesaje);

    Task EliminarPesaje(int idPesaje);
}

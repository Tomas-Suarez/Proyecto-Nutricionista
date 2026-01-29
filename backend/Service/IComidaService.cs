using backend.Dtos.Common;
using backend.Dtos.request;
using backend.Dtos.response;

namespace backend.Service;

public interface IComidaService
{
    Task<ComidaResponseDTO> RegistrarComida(ComidaRequestDTO dto);
    Task<PagedResponseDTO<ComidaResponseDTO>> ObtenerTodas(int page, int size, int? idCategoria, string? nombre);
    Task<ComidaResponseDTO> ObtenerPorId(int idComida);
    Task EliminarComida(int idComida);
    Task<ComidaResponseDTO> ActualizarComida(int idComida, ComidaRequestDTO dto);
}

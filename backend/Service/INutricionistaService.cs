using backend.Dtos.request;
using backend.Dtos.response;

namespace backend.Service;

public interface INutricionistaService
{
        Task<NutricionistaResponseDTO> RegistrarNutricionista(RegistroNutricionistaDTO dto);
        
        Task<NutricionistaResponseDTO> ObtenerNutricionistaPorId(int idNutricionista);

        Task<NutricionistaResponseDTO> ObtenerPorUsuarioId(int idUsuario);

        Task<NutricionistaResponseDTO> ModificarNutricionista(int IdNutricionista, NutricionistaRequestDTO dto);
}

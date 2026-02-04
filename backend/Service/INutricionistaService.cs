using backend.Dtos.request;
using backend.Dtos.response;

namespace backend.Service;

public interface INutricionistaService
{
        Task<NutricionistaResponseDTO> RegistrarNutricionista(RegistroNutricionistaDTO dto);
        
        Task<NutricionistaResponseDTO> ObtenerMiPerfil();

        Task<NutricionistaResponseDTO> ModificarMiPerfil(NutricionistaRequestDTO dto);
}

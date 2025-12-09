using backend.Dtos.request;
using backend.Dtos.response;

namespace backend.Service
{
    public interface IUsuarioService
    {
        Task<UsuarioResponseDTO> RegistrarUsuario(UsuarioRequestDTO dto);
    }
}

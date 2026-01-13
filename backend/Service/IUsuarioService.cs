using backend.Dtos.request;
using backend.Dtos.response;

namespace backend.Service
{
    public interface IUsuarioService
    {
        Task<UsuarioResponseDTO> RegistrarUsuario(UsuarioRequestDTO dto);

        Task<UsuarioResponseDTO> LoguearUsuario(UsuarioRequestDTO dto);

        Task<UsuarioResponseDTO> CambiarPassword(int idUsuario, CambiarPasswordRequestDTO dto);
        
        Task<UsuarioResponseDTO> ObtenerUsuarioPorId(int idUsuario);
    }
}

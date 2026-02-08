using backend.Dtos.request;
using backend.Dtos.response;

namespace backend.Service
{
    public interface IUsuarioService
    {
        Task<UsuarioResponseDTO> RegistrarUsuario(UsuarioRequestDTO dto);

        Task<LoginResponseDTO> LoguearUsuario(UsuarioRequestDTO dto);
        
        Task<LoginResponseDTO> RefrescarToken(string refreshTokenActual);

        Task<UsuarioResponseDTO> CambiarMiPassword(CambiarPasswordRequestDTO dto);
        
        Task<UsuarioResponseDTO> ObtenerUsuarioPorId(int idUsuario);

        Task<UsuarioResponseDTO> SubirAvatar(int idUsuario, IFormFile archivo);

        Task<UsuarioResponseDTO> BorrarAvatar(int idUsuario);
    }
}

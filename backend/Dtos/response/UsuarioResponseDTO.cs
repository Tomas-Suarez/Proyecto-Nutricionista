using backend.Enum;

namespace backend.Dtos.response
{
    public record UsuarioResponseDTO(
        int Id_Usuario,
        string Email,
        ERol Rol
    );
}
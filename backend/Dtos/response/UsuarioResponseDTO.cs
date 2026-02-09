using backend.Enum;

namespace backend.Dtos.response
{
    public record UsuarioResponseDTO
    {
        public int Id_Usuario { get; init; }
        public required string Email { get; init; }
        public ERol Rol { get; init; }
        public string? AvatarUrl { get; init; }
    }
}
namespace backend.Dtos.response;

public record LoginResponseDTO(
    UsuarioResponseDTO Usuario,
    string Token,
    string RefreshToken
);

namespace backend.Dtos.response;

public record PacienteResponseDTO(
    int Id_Paciente,
    int Id_Usuario,
    string NombreCompleto,
    string Dni,
    string Email,
    string? Telefono,
    decimal Altura_Cm
);

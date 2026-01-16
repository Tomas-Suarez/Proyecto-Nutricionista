namespace backend.Dtos.response;

public record NutricionistaResponseDTO(
    int Id_Nutricionista,
    int Id_Usuario,
    string NombreCompleto,
    string Matricula,
    string Telefono,
    string Email
);

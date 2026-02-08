namespace backend.Dtos.response;

public record NutricionistaResponseDTO(
    int Id_Nutricionista,
    int Id_Usuario,
    string Nombre,
    string Apellido,
    string Matricula,
    string Telefono,
    string Email
);

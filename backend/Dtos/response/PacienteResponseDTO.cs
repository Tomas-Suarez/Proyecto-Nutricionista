namespace backend.Dtos.response;

public record PacienteResponseDTO(
    int Id_Paciente,
    int Id_Usuario,    
    string Nombre,
    string Apellido,
    string Dni,
    string Email,
    string? AvatarUrl,
    string? Telefono,
    string? Genero,
    decimal Peso_Inicial,
    decimal Altura_Cm,
    string Estado,
    ObjetivoResponseDTO? Objetivo, 
    List<PatologiaResponseDTO> Patologias
);
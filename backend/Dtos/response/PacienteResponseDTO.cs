namespace backend.Dtos.response;

public record PacienteResponseDTO(
    int Id_Paciente,
    string Nombre,
    string Apellido,
    string Dni,
    string Email,
    string? Telefono,
    string? Genero,
    decimal Peso_Inicial,
    decimal Altura_Cm,
    string Estado,
    string? TokenAcceso,
    string? CodigoAcceso,
    decimal PesoActual,
    decimal Imc,
    ObjetivoResponseDTO? Objetivo, 
    List<PatologiaResponseDTO> Patologias,

    List<ArchivoResponseDTO>? ArchivosNutricionista = null,
    List<PesajeResponseDTO>? HistorialPeso = null,
    DietaResponseDTO? DietaActual = null
);
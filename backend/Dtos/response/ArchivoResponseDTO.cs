namespace backend.Dtos.response;

public record ArchivoResponseDTO(
    int Id_Archivo,
    string Nombre,
    string Url,
    DateTime Fecha
);
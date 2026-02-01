namespace backend.Dtos.response;

public record DietaResponseDTO(
    int Id_Dieta = 0,
    string Nombre = "",
    string PacienteNombre = "",
    decimal TotalCalorias = 0,
    decimal TotalProteinas = 0,
    decimal TotalCarbohidratos = 0,
    decimal TotalGrasas = 0,
    List<DietaComidaResponseDTO>? ComidasDetalle = null
);
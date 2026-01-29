namespace backend.Dtos.response;

public record DietaResponseDTO(
    int Id_Dieta,
    string Nombre,
    string PacienteNombre,
    decimal TotalCalorias,
    decimal TotalProteinas,
    decimal TotalCarbohidratos,
    decimal TotalGrasas,
    List<DietaComidaResponseDTO> ComidasDetalle
);

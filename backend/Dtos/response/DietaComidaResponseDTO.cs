namespace backend.Dtos.response;

public record DietaComidaResponseDTO(
    int Id_Dieta_Comida,
    int Id_Comida,
    string NombreComida,
    decimal Cantidad,
    string Horario,
    string? Nota,
    
    int CaloriasProporcionales,
    decimal ProteinasProporcionales,
    decimal CarbohidratosProporcionales,
    decimal GrasasProporcionales
);

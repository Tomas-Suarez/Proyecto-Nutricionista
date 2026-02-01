namespace backend.Dtos.response;

public record DietaComidaResponseDTO(
    int Id_Dieta_Comida = 0,
    int Id_Comida = 0,
    string NombreComida = "",
    decimal Cantidad = 0,
    string Horario = "",
    string? Nota = null,
    int CaloriasProporcionales = 0,
    decimal ProteinasProporcionales = 0,
    decimal CarbohidratosProporcionales = 0,
    decimal GrasasProporcionales = 0
);
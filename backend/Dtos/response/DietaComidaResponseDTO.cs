namespace backend.Dtos.response;

public record DietaComidaResponseDTO(
    int Id_Dieta_Comida,
    int Id_Comida,
    string NombreComida,
    string Cantidad,
    bool Es_Permitido,
    int Dia,
    string Momento
);
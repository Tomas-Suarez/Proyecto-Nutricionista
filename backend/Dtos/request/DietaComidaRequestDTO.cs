namespace backend.Dtos.request;

public record DietaComidaRequestDTO(
    int Id_Comida,
    decimal Cantidad,
    string Horario,
    string? Nota
);

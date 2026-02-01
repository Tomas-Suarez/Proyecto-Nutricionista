using backend.Enum;

namespace backend.Dtos.request;

public record DietaComidaRequestDTO(
    int Id_Comida = 0,
    decimal Cantidad = 0,
    EHorarioComida Horario = EHorarioComida.Almuerzo,
    string? Nota = null
);

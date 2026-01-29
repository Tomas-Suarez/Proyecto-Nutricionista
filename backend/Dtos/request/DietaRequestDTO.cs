namespace backend.Dtos.request;

public record DietaRequestDTO(
    int Id_Paciente,
    int Id_Nutricionista,
    string Nombre,
    string Descripcion,
    DateTime Fecha_Inicio,
    DateTime? Fecha_Fin,
    List<DietaComidaRequestDTO> Comidas
);

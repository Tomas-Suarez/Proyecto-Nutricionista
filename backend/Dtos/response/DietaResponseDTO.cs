using backend.Dtos.response;

public record DietaResponseDTO(
    int Id_Dieta,
    string Nombre,
    string Descripcion,
    DateTime Fecha_Inicio,
    DateTime Fecha_Fin,
    string Estado,
    string PacienteNombre,
    List<DietaComidaResponseDTO> Comidas
);
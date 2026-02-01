namespace backend.Dtos.request;

public record DietaRequestDTO(
int Id_Paciente = 0,
    int Id_Nutricionista = 0,
    string Nombre = "",
    string Descripcion = "",
    DateTime Fecha_Inicio = default,
    DateTime Fecha_Fin = default,
    List<DietaComidaRequestDTO> Comidas = null!
);

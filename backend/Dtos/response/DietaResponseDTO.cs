namespace backend.Dtos.response;

public record DietaResponseDTO
{
    public int Id_Dieta { get; init; }
    public int Id_Paciente { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string Descripcion { get; init; } = string.Empty;
    public DateTime Fecha_Inicio { get; init; }
    public DateTime Fecha_Fin { get; init; }
    public bool Activa { get; init; }
    public string PacienteNombre { get; init; } = string.Empty;
    public List<DietaComidaResponseDTO> Comidas { get; init; } = new();
}
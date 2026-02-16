namespace backend.Dtos.response;

public record DietaComidaResponseDTO
{
    public int Id_Comida { get; init; }
    public string NombreComida { get; init; } = string.Empty;
    public string NombreCategoria { get; init; } = string.Empty;
    public string Cantidad { get; init; } = string.Empty;
    public bool Es_Permitido { get; init; }
    public int Dia { get; init; }
    public string Momento { get; init; } = string.Empty;
}
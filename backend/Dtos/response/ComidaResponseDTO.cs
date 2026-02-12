namespace backend.Dtos.response;

public record ComidaResponseDTO(
    int Id_Comida,
    string Nombre,
    string? Descripcion,
    decimal Calorias,
    decimal Proteinas,
    decimal Carbohidratos,
    decimal Grasas,
    decimal? Fibra,
    decimal? Azucares,
    string Porcion,
    string? Imagen_Url,
    List<CategoriaResponseDTO> Categorias
);

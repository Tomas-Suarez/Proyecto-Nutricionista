using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.request;

public record ComidaRequestDTO(
    
    [Required(ErrorMessage = "El nombre de la comida es obligatorio")]
    [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres")]
    string Nombre,

    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres")]
    string? Descripcion,

    [Required(ErrorMessage = "Las proteínas son obligatorias")]
    [Range(0, 1000, ErrorMessage = "Las proteínas deben ser un valor entre 0 y 1000")]
    decimal Proteinas,

    [Required(ErrorMessage = "Los carbohidratos son obligatorios")]
    [Range(0, 1000, ErrorMessage = "Los carbohidratos deben ser un valor entre 0 y 1000")]
    decimal Carbohidratos,

    [Required(ErrorMessage = "Las grasas son obligatorias")]
    [Range(0, 1000, ErrorMessage = "Las grasas deben ser un valor entre 0 y 1000")]
    decimal Grasas,

    [Range(0, 1000, ErrorMessage = "La fibra debe ser un valor positivo")]
    decimal? Fibra,

    [Range(0, 1000, ErrorMessage = "Los azúcares deben ser un valor positivo")]
    decimal? Azucares,

    [Required(ErrorMessage = "La porción de referencia es obligatoria")]
    [StringLength(50, ErrorMessage = "La porción no puede superar los 50 caracteres")]
    string Porcion,

    [Url(ErrorMessage = "La URL de la imagen no es válida")]
    string? Imagen_Url,

    [Required(ErrorMessage = "Debes seleccionar al menos una categoría")]
    [MinLength(1, ErrorMessage = "La comida debe estar vinculada a al menos una categoría")]
    List<int> Ids_Categorias

);

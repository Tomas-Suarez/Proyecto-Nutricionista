using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.request;

public record NutricionistaRequestDTO(

    [Required(ErrorMessage = "El ID de usuario es obligatorio.")]
    int Id_Usuario,
    
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    string Nombre,

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres.")]
    string Apellido,

    [Required(ErrorMessage = "La matrícula es obligatoria.")]
    [StringLength(50, ErrorMessage = "La matrícula no puede superar los 50 caracteres.")]
    string Matricula,

    [Required(ErrorMessage = "El formato del teléfono no es válido.")]
    [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
    string Telefono
);

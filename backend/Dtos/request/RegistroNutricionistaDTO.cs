using System.ComponentModel.DataAnnotations;
using backend.Enum;

namespace backend.Dtos.request;

public record RegistroNutricionistaDTO(
    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
    string Email,

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    string Password,

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    string Nombre,

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres.")]
    string Apellido,

    [Required(ErrorMessage = "La matrícula es obligatoria.")]
    [StringLength(50, ErrorMessage = "La matrícula no puede superar los 50 caracteres.")]
    string Matricula,

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
    [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
    string Telefono
);
using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.request;

public record RegistroPacienteDTO(
    
    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
    string Email,

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    string Nombre,

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres.")]
    string Apellido,

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [StringLength(50, ErrorMessage = "El DNI no puede superar los 50 caracteres.")]
    string Dni,

    [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
    string? Telefono,

    [Required(ErrorMessage = "El género es obligatorio.")]
    [StringLength(20, ErrorMessage = "El género no puede superar los 20 caracteres.")]
    string Genero,

    [Required(ErrorMessage = "El peso inicial es obligatorio.")]
    [Range(1, 500, ErrorMessage = "El peso debe estar entre 1 y 500 kg.")]
    decimal Peso_Inicial,

    [Required(ErrorMessage = "La altura es obligatoria.")]
    [Range(30, 300, ErrorMessage = "La altura debe estar entre 30 y 300 cm.")]
    decimal Altura_Cm,
    int? IdObjetivo,

    List<int>? IdsPatologias
    
);
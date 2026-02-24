using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.request;

public record PacienteRequestDTO(
    
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    string Nombre,

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres.")]
    string Apellido,

    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    [StringLength(100, ErrorMessage = "El email no puede superar los 100 caracteres.")]
    string? Email,

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [StringLength(50, ErrorMessage = "El DNI no puede superar los 50 caracteres.")]
    string Dni,

    [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
    string? Telefono,

    [Required(ErrorMessage = "El género es obligatorio.")]
    [StringLength(20, ErrorMessage = "El género no puede superar los 20 caracteres.")]
    string Genero,

    [Required(ErrorMessage = "La altura es obligatoria.")]
    decimal Altura_Cm,

    [Required(ErrorMessage = "El peso inicial es obligatorio.")]
    [Range(0.1, 500, ErrorMessage = "El peso debe ser un valor válido.")]
    decimal Peso_Inicial,

    int? Id_Objetivo,
    List<int> IdsPatologias
);
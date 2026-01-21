using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.request;

public record CategoriaRequestDTO(
    
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(30, ErrorMessage = "El nombre no puede superar los 30 caracteres.")]
    string Nombre
);

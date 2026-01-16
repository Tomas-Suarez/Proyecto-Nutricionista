using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.request;

public record CambiarPasswordRequestDTO(
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        string PasswordActual,
        
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        string PasswordNueva
);
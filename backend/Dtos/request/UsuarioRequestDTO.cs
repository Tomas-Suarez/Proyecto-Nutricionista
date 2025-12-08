using backend.Enum;
using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.request
{
    public record UsuarioRequestDTO(
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        string Email,

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        string Password,

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [EnumDataType(typeof(ERol), ErrorMessage = "El rol no es válido.")]
        ERol Rol

        // TODO: quedara para el final
        //IFormFile? AvatarFile
    );
}
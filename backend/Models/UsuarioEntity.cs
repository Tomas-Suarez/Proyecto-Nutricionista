using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Usuario")]
    public class UsuarioEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_Usuario { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password_Hash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [RegularExpression("Nutricionista|Paciente", ErrorMessage = "El rol debe ser 'Nutricionista' o 'Paciente'")]
        public string Rol { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Avatar_Url { get; set; }
    }
}
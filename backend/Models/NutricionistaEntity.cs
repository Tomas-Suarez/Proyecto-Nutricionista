using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("Nutricionista")]
public class NutricionistaEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id_Nutricionista { get; set; }

    [Required]
    public int Id_Usuario { get; set; }

    [ForeignKey("Id_Usuario")]
    public virtual UsuarioEntity Usuario { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Apellido { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Matricula { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Telefono { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("Archivos_Nutricionista")]
public class ArchivoNutricionistaEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id_Archivo { get; set; }

    [Required]
    public int Id_Nutricionista { get; set; }

    [ForeignKey("Id_Nutricionista")]
    public virtual NutricionistaEntity Nutricionista { get; set; } = null!;

    [Required]
    public string NombreArchivo { get; set; } = string.Empty;

    [Required]
    public string RutaAcceso { get; set; } = string.Empty;

    public DateTime FechaSubida { get; set; } = DateTime.Now;   
}
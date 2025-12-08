using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("Comida")]
public class ComidaEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id_Comida { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Descripcion { get; set; }

    [Required]
    public int Calorias { get; set; }

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal Proteinas { get; set; }

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal Carbohidratos { get; set; }

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal Grasas { get; set; }

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal Fibra { get; set; }

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal Azucares { get; set; }

    [Required]
    [StringLength(50)]
    public string Porcion { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Imagen_Url { get; set; }
}
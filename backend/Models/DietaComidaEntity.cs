using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("Dieta_Comida")]
public class DietaComidaEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id_Dieta_Comida { get; set; }

    [Required]
    public int Id_Dieta { get; set; }

    [ForeignKey("Id_Dieta")]
    public virtual DietaEntity Dieta { get; set; } = null!;

    [Required]
    public int Id_Comida { get; set; }

    [ForeignKey("Id_Comida")]
    public virtual ComidaEntity Comida { get; set; } = null!;

    [Required]
    public decimal Cantidad { get; set; }

    [Required]
    [StringLength(50)]
    public string Horario { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Nota { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Enum;

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

    public required string Cantidad { get; set; }

    public bool Es_Permitido { get; set; } = true;

    public int Dia { get; set; }

    public EMomento Momento { get; set; }
}
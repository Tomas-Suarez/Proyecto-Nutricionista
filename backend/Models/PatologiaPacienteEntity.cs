using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("Patologia_Paciente")]
public class PatologiaPacienteEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id_Patologia_Paciente { get; set; }

    [Required]
    public int Id_Patologia { get; set; }


    [ForeignKey("Id_Patologia")]
    public virtual PatologiaEntity Patologia { get; set; } = null!;

    [Required]
    public int Id_Paciente { get; set; }


    [ForeignKey("Id_Paciente")]
    public virtual PacienteEntity Paciente { get; set; } = null!;
}

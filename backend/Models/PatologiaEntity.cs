using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("Patologia")]
public class PatologiaEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id_Patologia { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;

    public virtual ICollection<PatologiaPacienteEntity> PatologiaPacientes { get; set; } = new List<PatologiaPacienteEntity>();

}

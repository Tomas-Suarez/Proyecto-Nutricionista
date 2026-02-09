using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("Objetivo")]
public class ObjetivoEntity
{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_Objetivo { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        public virtual ICollection<PacienteEntity> Pacientes { get; set; } = new List<PacienteEntity>();
}

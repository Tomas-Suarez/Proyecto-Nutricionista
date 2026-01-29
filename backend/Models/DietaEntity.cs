using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Dieta")]
    public class DietaEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_Dieta { get; set; }

        [Required]
        public int Id_Paciente { get; set; }

        [ForeignKey("Id_Paciente")]
        public virtual PacienteEntity Paciente { get; set; } = null!;

        [Required]
        public int Id_Nutricionista { get; set; }

        [ForeignKey("Id_Nutricionista")]
        public virtual NutricionistaEntity Nutricionista { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public DateTime Fecha_Inicio { get; set; }

        public DateTime? Fecha_Fin { get; set; }

        [Required]
        public bool Activa {  get; set; } = false;

        public virtual ICollection<DietaComidaEntity> DietaComidas { get; set; } = new List<DietaComidaEntity>();
    }
}
